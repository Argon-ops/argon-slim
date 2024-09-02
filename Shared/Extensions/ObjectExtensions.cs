
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine;

namespace DuksGames.Argon.Shared
{
    public static class ObjectExtensions
    {

        public static string Dump(this System.Object _this)
        {
            return _this.GetType().GetFields()
                .Select(info => (info.Name, Value: info.GetValue(_this) ?? "(null)"))
                .Aggregate(
                    new StringBuilder(),
                    (sb, pair) =>
                    {
                        if (pair.Value.GetType().IsArray)
                        {
                            sb.Append($"{pair.Name}: [");
                            if (pair.Value is IEnumerable enumeration)
                                foreach (var item in enumeration)
                                {
                                    sb.AppendLine($"'{item.ToString()}'");
                                }

                            return sb.AppendLine("]");
                        }

                        return sb.AppendLine($"{pair.Name}: {pair.Value.ToString()} ");
                    },
                    sb => sb.ToString());
        }

        static void _DumpRecursive(System.Object _this, HashSet<System.Object> already, StringBuilder sb, int indent = 0)
        {
            if (_this.GetType().IsPrimitive)
            {
                sb.AppendLine($"{new string(' ', indent * 2)}{_this}");
                return;
            }
            if (already.Contains(_this))
            {
                sb.AppendLine($"<previously-encountered-object: {_this.ToString()}>");
                return;
            }
            already.Add(_this);

            var tabs = new string(' ', indent * 2);

            foreach (var info in _this.GetType().GetFields())
            {
                var val = info.GetValue(_this);
                if (val == null)
                {
                    sb.AppendLine($"{tabs}{info.Name}: (null)");
                    continue;
                }

                if (val.GetType().IsArray)
                {
                    sb.Append($"{tabs}{info.Name}: [\n");
                    if (val is IEnumerable enumeration)
                    {
                        foreach (var item in enumeration)
                        {
                            ObjectExtensions._DumpRecursive(item, already, sb, indent + 1);
                        }
                    }
                    sb.AppendLine($"{tabs}]");
                    continue;
                }

                if (val.GetType().IsEnum)
                {
                    sb.AppendLine($"{tabs}{info.Name}: {val.ToString()}");
                    continue;
                }

                if (val.GetType().IsClass || (val.GetType().IsValueType && !val.GetType().IsPrimitive))
                {
                    sb.Append($"{tabs}{info.Name}: {{ \n");
                    ObjectExtensions._DumpRecursive(val, already, sb, indent + 1);

                    sb.AppendLine($"{tabs}}}");
                    continue;
                }

                sb.AppendLine($"{tabs}{info.Name}: {val.ToString()}");
            }
        }

        public static string DumpRecursive(this System.Object _this)
        {
            var hs = new HashSet<object>();
            var sb = new StringBuilder();
            ObjectExtensions._DumpRecursive(_this, hs, sb);
            return sb.ToString();
        }

        /// <summary>
        /// Lord of Duct https://forum.unity.com/threads/another-null-ref-thread.523760/
        /// Returns true if the object is either a null reference or has been destroyed by unity.
        /// This will respect ISPDisposable over all else.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;
           
            if(obj is UnityEngine.Object) return (obj as UnityEngine.Object) == null;
 
            return false;
        }
    }
}