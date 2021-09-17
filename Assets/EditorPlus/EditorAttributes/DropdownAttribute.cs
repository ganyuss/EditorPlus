using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(EditorPlusAttribute.AttributeDrawerTargets)]
    public class DropdownAttribute : PropertyAttribute {
        public object DropdownElements;

        public DropdownAttribute(object elements) {
            DropdownElements = elements;
        }
    }

    public abstract class DropdownList {

        public abstract string[] GetLabels();
        public abstract object[] GetValues();
    }

    public class DropdownList<Element> : DropdownList {
        private List<string> Labels = new List<string>();
        private List<Element> Values = new List<Element>();

        public void AddEntry(string label, Element element) {
            Labels.Add(label);
            Values.Add(element);
        }

        public Element this[string label] {
            get => Values[Labels.IndexOf(label)];
            set {
                if (Labels.Contains(label)) {
                    Values[Labels.IndexOf(label)] = value;
                }
                else {
                    AddEntry(label, value);
                }
            }
        }

        public override string[] GetLabels() {
            return Labels.ToArray();
        }

        public override object[] GetValues() {
            return Values.Select(e => (object)e).ToArray();
        }
    }
}
