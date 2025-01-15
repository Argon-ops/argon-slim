using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using DuksGames.Argon.Shared;
using UnityEditor.AssetImporters;
using NUnit.Framework;

namespace DuksGames.Tools
{
    public class PostProcessorRouter : AssetPostprocessor
    {

        List<AbstractCustomPropProcessor> _persistedProcessors = new();

        IntermediateProductSet _intermediateProductSet = new();

        ImportHierarchyLookup _importHierarchyLookup = new();

        DefaultKeySets _cachedDefaultKeySets;
        DefaultKeySets GetDefaultKeySets()
        {
            if (_cachedDefaultKeySets == null)
            {
                _cachedDefaultKeySets = new DefaultKeySets();
            }
            return _cachedDefaultKeySets;
        }

        IEnumerable<IProcessorKeySet> GetKeySets()
        {
            foreach (var ks in this.GetDefaultKeySets().KeySets)
            {
                yield return ks;
            }
        }

        bool KeyHasProcessor(string key, out IProcessorKeySet keySet)
        {
            foreach (var _keySet in this.GetKeySets())
            {
                if (_keySet.IsTargetKey(key))
                {
                    keySet = _keySet;
                    return true;
                }
            }
            keySet = null;
            return false;
        }

        ModelImporter _modelImporter => (ModelImporter)this.assetImporter;

        void DForceIsReadable()
        {
            if (!this._modelImporter.isReadable)
            {
                // TODO: remember why we'd ever care about isReadable
                this._modelImporter.isReadable = true;
                this.assetImporter.SaveAndReimport();
            }
        }

        #region unity-import-methods

        void OnPreprocessModel()
        {
            this.DForceIsReadable();
            this._persistedProcessors.Clear();
            this._intermediateProductSet.Clear();
            CommandLookup.Instance.Clear();
            this._importHierarchyLookup.Clear();
        }

        bool IsBlenderFile()
        {
            return System.IO.Path.GetExtension(this.assetPath).ToLower() == ".blend";
        }

        void OnPostprocessGameObjectWithUserProperties(
            GameObject go,
            string[] propNames,
            System.Object[] values)
        {

            Logger.ImportLog("##### IMPORT LOG TESET> DEL ME ******######");

            if(this.IsBlenderFile())
            {
                // Decline to touch the file if it is .blend
                //  Unity does import blender files as fbx but not always with the settings that we expect
                //    and not with all of the Argon properties in all cases.
                return;
            }

            if (!StoreArgonPreferences.IsArgonEnabled())
            {
                return;
            }

            var info = new CustomPropApplyInfo()
            {
                Target = go,
                Postprocessor = this,
            };

            var uniqueProcessorKeySets = this.GetDefaultKeySets().UniquePerObjectKeySets.ToList();
            // iterate over the keys to create all the interpreters we need
            var processors = new Dictionary<string, AbstractCustomPropProcessor>();
            for (int i = 0; i < propNames.Length; ++i)
            {
                if (this.KeyHasProcessor(propNames[i], out IProcessorKeySet keySet))
                {
                    if (keySet.ExcludeObject(propNames[i], go))
                        continue;
                    
                    processors.Add(propNames[i], keySet.CreateProcessor(info));
                    continue;
                }

                // Allow unique processors to see the key
                for(int j=0; j<uniqueProcessorKeySets.Count; ++j)
                {
                    var unique = uniqueProcessorKeySets[j];
                    if(unique.IsTargetKey(propNames[i]))
                    {
                        processors.Add(propNames[i], unique.CreateProcessor(info));
                        uniqueProcessorKeySets.RemoveAt(j); // remove the keyset to stop it from making another processor
                        break;
                    }
                }

            }

            // second iteration to let the interpreters gather their data
            for (int i = 0; i < propNames.Length; ++i)
            {
                foreach (var processor in processors.Values)
                {
                    if (processor.ClaimKey(propNames[i], values[i]))
                    {
                        continue;
                    }
                }
            }

            // apply
            foreach (var processor in processors.Values)
            {
                if (processor is IApplyCustomProperties properties)
                {
                    properties.Apply();
                }
            }

            // store the processors: some of them may want to do more work on their targets later on, in OnPostprocessModel, etc.
            this._persistedProcessors.AddRange(processors.Values);

            this._importHierarchyLookup.Add(go);
        }

        void OnPostprocessAnimation(GameObject go, AnimationClip clip)
        {

            if(this.IsBlenderFile())
            {
                return;
            }

            if (!StoreArgonPreferences.IsArgonEnabled())
            {
                return;
            }

            this._intermediateProductSet.AddClip(clip);

            foreach (var processor in this._persistedProcessors)
            {
                if (processor is IAnimationClipPostProcessor postProcessor)
                {
                    postProcessor.PostProcessAnimation(go, clip);
                }
            }

        }


        void OnPostprocessModel(GameObject root)
        {
            try
            {
                this._OnPostProcessModel(root);
            }
            catch(System.Exception)
            {
                Debug.LogWarning($"On PostProcessModel will throw for root gameobject: {root}");
                throw;
                // throw new System.Exception($"On Postprocessmodel with root gameobject: {root} | {e.Message}");
            }
        }

        void _OnPostProcessModel(GameObject root)
        {
            if(this.IsBlenderFile())
            {
                return;
            }

            if (!StoreArgonPreferences.IsArgonEnabled())
            {
                return;
            }

            var mppi = new ModelPostProcessInfo
            {
                Root = root,
                AssetPostprocessor = this,
                ImportHierarchyLookup = this._importHierarchyLookup,
                AssociateLookup = new(),
            };

            foreach (var processor in this._persistedProcessors.OfType<IModelPostProcessor>())
            {
                ((IModelPostProcessor)processor).PostProcessModel(mppi);
            }

            foreach (var processor in this._persistedProcessors.OfType<IIntermediateProductProducer>())
            {
                processor.SetProductSet(mppi, this._intermediateProductSet);
            }

            foreach (var processor in this._persistedProcessors.OfType<IIntermediateProductConsumer>())
            {
                ((IIntermediateProductConsumer)processor).Consume(mppi, this._intermediateProductSet);
            }

            foreach (var globalsConsumer in GlobalsConsumersList.Consumers)
            {
                globalsConsumer.ConsumeGlobals(mppi.Root, this._intermediateProductSet);
            }

            foreach (var linker in this._persistedProcessors.OfType<ILinkerProcess>())
            {
                ((ILinkerProcess)linker).Link(mppi);
            }

            this.DoLinkers();

            foreach (var cleanup in this._persistedProcessors.OfType<ICleanupProcess>())
            {
                cleanup.Cleanup(mppi);
            }

            this._persistedProcessors.Clear(); 

        }

        public void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] materialAnimation)
        {
            if (!StoreArgonPreferences.IsArgonEnabled())
            {
                return;
            }
        }

        #endregion

        void DoLinkers()
        {
            CommandFactory.LinkerPass(this._intermediateProductSet);
        }

    }

}