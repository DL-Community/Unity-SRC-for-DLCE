// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Unity.UI.Builder
{
    internal class FoldoutField : PersistedFoldout
    {
        public new class UxmlFactory : UxmlFactory<FoldoutField, UxmlTraits> {}

        public new class UxmlTraits : PersistedFoldout.UxmlTraits
        {
            UxmlStringAttributeDescription m_BindingPaths = new UxmlStringAttributeDescription { name = "binding-paths" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var separator = ' ';
                ((FoldoutField)ve).bindingPathArray = m_BindingPaths.GetValueFromBag(bag, cc).Split(separator);

                ((FoldoutField)ve).ReAssignTooltipToHeaderLabel();
            }
        }

        protected string[] m_BindingPathArray;

        public string[] bindingPathArray
        {
            get
            {
                return m_BindingPathArray;
            }
            set
            {
                m_BindingPathArray = value;
            }
        }

        public FoldoutField()
        {
            m_Value = true;
            AddToClassList(BuilderConstants.FoldoutFieldPropertyName);
            header.AddToClassList(BuilderConstants.FoldoutFieldHeaderClassName);
        }

        public virtual void UpdateFromChildFields() {}
    }
}
