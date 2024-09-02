using System.Collections.Generic;
using DuksGames.Argon.Core;
using UnityEngine;
using TMPro;

namespace DuksGames.Tools
{
    public class TextMeshKeySet : AbstractCustomPropKeySet<TextMeshProcessor>
    {
        public override string TargetKey => "mel_text_mesh";

        public override IEnumerable<string> GetKeys()
        {
            yield return this.AppendSuffix("_content");
            yield return this.AppendSuffix("_use_tmpro");

            yield return this.AppendSuffix("_offset_z");
            yield return this.AppendSuffix("_character_size");
            yield return this.AppendSuffix("_alignment");
            yield return this.AppendSuffix("_anchor");
            yield return this.AppendSuffix("_font_size");
            yield return this.AppendSuffix("_always_face_camera");
            yield return this.AppendSuffix("_enable_radius");
            yield return this.AppendSuffix("_color");
            yield return this.AppendSuffix("_rotate_degrees_y");

            // TMPro settings
            yield return this.AppendSuffix("_overflow");
            yield return this.AppendSuffix("_wrapping");
            yield return this.AppendSuffix("_horizontal_alignment");
            yield return this.AppendSuffix("_vertical_alignment");
            yield return this.AppendSuffix("_font_name");

        }

    }

    public class TextMeshProcessor : AbstractCustomPropProcessor, IApplyCustomProperties
    {
        // TODO: disentangle text mesh like. it adds component but then hides it with proximity. never unhides.
        //   TODO: let us opt into the proximity with text mesh.

        public void Apply()
        {
            var target = this.CreateTextObject();
            this.Billboard(target);
            this.AddProximity(target);
        }

        GameObject CreateTextObject()
        {
            if (this.GetBoolWithSuffix("_use_tmpro", true))
            {
                return this.ConfigureTextPro();
            }
            return this.ConfigureText();
        }

        GameObject ConfigureTextPro()
        {
            var target = new GameObject(this.ApplyInfo.Target.name + "_TextMesh");
            target.transform.position = this.ApplyInfo.Target.transform.position;

            target.transform.SetParent(this.ApplyInfo.Target.transform);

            var tm = target.AddComponent<TextMeshPro>();
            tm.text = this.GetStringWithSuffix("_content");

            // var fontName = this.GetStringWithSuffix("_font_name");
            // if(!string.IsNullOrEmpty(fontName)) {
            //   TODO: TMPro font: this doesn't work but maybe it was the particular import file we were working with
            //     Try on another ble file
            //     var font = MelGameObjectHelper.FindInProject<Font>(fontName);
            //     // tm.font = TMP_FontAsset.CreateFontAsset(font);
            // }

            tm.enableWordWrapping = this.GetBoolWithSuffix("_wrapping", true);
            tm.overflowMode = (TextOverflowModes)this.GetIntWithSuffix("_overflow");
            tm.horizontalAlignment = (HorizontalAlignmentOptions)
                    System.Enum.GetValues(typeof(HorizontalAlignmentOptions)).GetValue(this.GetIntWithSuffix("_horizontal_alignment"));

            tm.verticalAlignment = (VerticalAlignmentOptions)
                    System.Enum.GetValues(typeof(VerticalAlignmentOptions)).GetValue(this.GetIntWithSuffix("_vertical_alignment"));

            // tm.offsetZ = this.GetFloatWithSuffix("_offset_z");
            // tm.characterSize = this.GetFloatWithSuffix("_character_size");
            // tm.alignment = (TextAlignment)this.GetIntWithSuffix("_alignment");
            // tm.anchor = (TextAnchor)this.GetIntWithSuffix("_anchor");
            tm.fontSize = this.GetFloatWithSuffix("_font_size");
            tm.color = this.GetColorWithSuffix("_color");

            target.transform.Rotate(Vector3.up * this.GetFloatWithSuffix("_rotate_degrees_y"), Space.World);

            // TEST TRY: size to bounds if available
            this.SizeToBounds(tm);

            // Need for fontSize
            tm.UpdateFontAsset();
            tm.ForceMeshUpdate();

            return target;
        }

        private void SizeToBounds(TextMeshPro tm)
        {
            var renderer = this.ApplyInfo.Target.GetComponent<Renderer>();
            if (!renderer) { return; }
            tm.rectTransform.sizeDelta = renderer.bounds.size;
        }

        GameObject ConfigureText()
        {
            var target = new GameObject(this.ApplyInfo.Target.name + "_TextMesh");
            target.transform.position = this.ApplyInfo.Target.transform.position;
            target.transform.SetParent(this.ApplyInfo.Target.transform);

            var tm = target.AddComponent<TextMesh>();
            tm.text = this.GetStringWithSuffix("_content");
            tm.offsetZ = this.GetFloatWithSuffix("_offset_z");
            tm.characterSize = this.GetFloatWithSuffix("_character_size");
            tm.alignment = (TextAlignment)this.GetIntWithSuffix("_alignment");
            tm.anchor = (TextAnchor)this.GetIntWithSuffix("_anchor");
            tm.fontSize = this.GetIntWithSuffix("_font_size");
            tm.color = this.GetColorWithSuffix("_color");

            target.transform.Rotate(Vector3.up * this.GetFloatWithSuffix("_rotate_degrees_y"), Space.World);
            return target;
        }

        void Billboard(GameObject target)
        {
            if (this.GetBoolWithSuffix("_always_face_camera", true))
            {
                var billboard = target.AddComponent<CameraFacingBillboard>();
            }
        }

        void AddProximity(GameObject target)
        {
            var proximity = target.AddComponent<SluggishProximity>();
            proximity.Radius = this.GetFloatWithSuffix("_enable_radius");
            var adapter = target.AddComponent<ProximityOmniAdapter>();
            proximity.CallbackLink = adapter;

            var positioner = target.AddComponent<ProximityGenericPositionProvider>();
            proximity.SluggishPositionLink = positioner;

            var componentOnOff = target.AddComponent<ComponentEnable>();
            componentOnOff.TypeName = "Renderer";

            adapter.IOnOffLinks = new Component[] { componentOnOff };
        }
    }
}