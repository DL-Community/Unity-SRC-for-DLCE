// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.GraphToolsFoundation.Editor
{
    /// <summary>
    /// Inspector for the serializable fields of a <see cref="GraphElementModel"/> or its surrogate, if it implements <see cref="IHasInspectorSurrogate"/>.
    /// Will Display in this order:
    /// - Fields without [MoveAfterInspectorAttribute] matching m_Filter.
    /// - Fields from GetCustomFields.
    /// - Ordered Fields With [MoveAfterInspectorAttribute] matching m_Filter.
    /// </summary>
    class SerializedFieldsInspector : FieldsInspector
    {
        const BindingFlags k_FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Determines if the field can be inspected. A field can be inspected if it is public or if it has the
        /// <see cref="SerializeField"/> attribute. In addition, it must not have the <see cref="HideInInspector"/>
        /// attribute nor the <see cref="NonSerializedAttribute"/> attribute.
        /// </summary>
        /// <param name="f">The field to inspect.</param>
        /// <returns>True if the field can be inspected, false otherwise.</returns>
        public static bool CanBeInspected(FieldInfo f)
        {
            if (f != null)
            {
                var isSerializable = (f.Attributes & FieldAttributes.Public) == FieldAttributes.Public ||
                    (f.Attributes & FieldAttributes.Private) == FieldAttributes.Private &&
                    f.GetCustomAttribute<SerializeField>() != null;
                isSerializable &= !f.IsNotSerialized;

                if (isSerializable
                    && f.GetCustomAttribute<HideInInspector>() == null
                    && f.GetCustomAttribute<ObsoleteAttribute>() == null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SerializedFieldsInspector"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="models">The models displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <param name="filter">A filter function to select which fields are displayed in the inspector. If null, defaults to <see cref="CanBeInspected"/>.</param>
        /// <returns>A new instance of <see cref="SerializedFieldsInspector"/>.</returns>
        public static SerializedFieldsInspector Create(string name, IReadOnlyList<Model> models, ChildView ownerElement,
            string parentClassName, Func<FieldInfo, bool> filter = null)
        {
            return new SerializedFieldsInspector(name, models, ownerElement, parentClassName, filter);
        }

        Func<FieldInfo, bool> m_Filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializedFieldsInspector"/> class.
        /// </summary>
        /// <param name="name">The name of the part.</param>
        /// <param name="models">The models displayed in this part.</param>
        /// <param name="ownerElement">The owner of the part.</param>
        /// <param name="parentClassName">The class name of the parent.</param>
        /// <param name="filter">A filter function to select which fields are displayed in the inspector. If null, defaults to <see cref="CanBeInspected"/>.</param>
        protected SerializedFieldsInspector(string name, IReadOnlyList<Model> models, ChildView ownerElement,
            string parentClassName, Func<FieldInfo, bool> filter)
            : base(name, models, ownerElement, parentClassName)
        {
            m_Filter = filter ?? CanBeInspected;
        }

        /// <summary>
        /// Gets the objects displayed by the inspector. It usually is the model passed to the constructor, unless the
        /// model implements <see cref="IHasInspectorSurrogate"/>, in which case it is the surrogate object.
        /// </summary>
        /// <returns>The inspected object.</returns>
        public IEnumerable<object> GetInspectedObjects()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global : IHasInspectorSurrogate is for use by clients.
            return m_Models.Select(t => t is IHasInspectorSurrogate surrogate ? surrogate.Surrogate: t);
        }

        /// <summary>
        /// Allow adding field to display between the default fields and the default field with a <see cref="InspectorFieldOrderAttribute"/>.
        /// </summary>
        /// <returns>An optional list of <see cref="BaseModelPropertyField"/>.</returns>
        protected virtual List<BaseModelPropertyField> GetCustomFields()
        {
            return null;
        }

        /// <inheritdoc />
        protected override IEnumerable<BaseModelPropertyField> GetFields()
        {
            var targets = GetInspectedObjects();

            if (targets == null)
                yield break;

            var inspectorOrderFields = new SortedDictionary<int, List<BaseModelPropertyField>>();

            foreach (var field in AddFieldsFromTypes(targets, inspectorOrderFields))
                yield return field;

            var customFields = GetCustomFields();
            if (customFields != null)
            {
                foreach (var field in customFields)
                    yield return field;
            }

            foreach (var fieldAtPositionList in inspectorOrderFields.Values)
            {
                foreach (var field in fieldAtPositionList)
                    yield return field;
            }
        }

        protected IEnumerable<BaseModelPropertyField> AddFieldsFromTypes(IEnumerable<object> targets, SortedDictionary<int, List<BaseModelPropertyField>> inspectorOrderFields)
        {
            var type = ModelHelpers.GetCommonBaseType(targets);

            if (type == null)
                yield break;

            var typeList = new List<Type>();

            while (type != null)
            {
                typeList.Insert(0, type);
                type = type.BaseType;
            }

            foreach (var t in typeList)
            {
                var fields = t.GetFields(k_FieldFlags);
                foreach (var fieldInfo in fields.Where(m_Filter))
                {
                    var moveAfter = fieldInfo.GetCustomAttribute<InspectorFieldOrderAttribute>();

                    if (moveAfter != null)
                    {
                        AddFieldToInspectorOrderFields(moveAfter.Order, GetFieldFromFieldInfo(fieldInfo), inspectorOrderFields);
                        continue;
                    }

                    yield return GetFieldFromFieldInfo(fieldInfo);
                }
            }

            BaseModelPropertyField GetFieldFromFieldInfo(FieldInfo fieldInfo1)
            {
                var tooltip = "";
                var tooltipAttribute = fieldInfo1.GetCustomAttribute<TooltipAttribute>();
                if (tooltipAttribute != null)
                {
                    tooltip = tooltipAttribute.tooltip;
                }

                var modelFieldFieldType = typeof(ModelSerializedFieldField_Internal<>).MakeGenericType(fieldInfo1.FieldType);
                var baseModelPropertyField = Activator.CreateInstance(
                        modelFieldFieldType, OwnerRootView, m_Models, targets, fieldInfo1, tooltip)
                    as BaseModelPropertyField;
                return baseModelPropertyField;
            }
        }

        protected static void AddFieldToInspectorOrderFields(int order, BaseModelPropertyField fieldToAdd, SortedDictionary<int, List<BaseModelPropertyField>> inspectorOrderFields)
        {
            if (!inspectorOrderFields.TryGetValue(order, out var fieldsAtPosition))
            {
                fieldsAtPosition = new List<BaseModelPropertyField>();
                inspectorOrderFields[order] = fieldsAtPosition;
            }
            fieldsAtPosition.Add(fieldToAdd);
        }
    }
}
