// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Declares a custom control.
    /// </summary>
    /// <remarks>
    /// To create a custom control, you must add the UxmlElement attribute to the custom control class definition.
    /// You must declare the custom control class as a partial class and inherit it from <see cref="VisualElement"/>
    /// or one of its derived classes.
    /// When an element is marked with the UxmlElement attribute, a corresponding <see cref="UxmlSerializedData"/>
    /// class is generated in the partial class.
    /// This data class contains a <see cref="SerializeField"/> for each field or property that was marked with the
    /// <see cref="UxmlAttributeAttribute"/> attribute.
    /// This serialized data allows for the element to be serialized from UXML and supports editing in the Attributes field of the
    /// Inspector window in  the UI Builder.
    /// By default, the custom control appears in the Library tab in UI Builder.
    /// To hide it from the Library tab, provide the <see cref="HideInInspector"/> attribute.
    /// </remarks>
    /// <example>
    /// The following example creates a custom control with multiple attributes:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlElement_ExampleElement.cs"/>
    /// </example>
    /// <example>
    /// The following UXML document uses the custom control:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlElement_ExampleElement.uxml"/>
    /// </example>
    /// <example>
    /// <para>When you create a custom control, the default name used in UXML and UI Builder is the element type name (C# class name).
    /// However, you can customize the name to make it easier to refer to the element.</para>
    /// <para>__Note__: You are still required to reference the classes' namespace in UXML.</para>
    /// <para>To create a custom name for an element, provide a value to the @@name@@ property.
    /// For example, if you create the following custom button:</para>
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlElement_CustomButtonElement.cs"/>
    /// </example>
    /// <example>
    /// You can then reference the custom button in UXML with the custom name or its type:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlElement_CustomButtonElement.uxml"/>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class UxmlElementAttribute : Attribute
    {
        /// <summary>
        /// Provides a custom name for an element.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Exposes a type of VisualElement to UXML and UI Builder
        /// </summary>
        public UxmlElementAttribute() : this(null)
        { }

        /// <summary>
        /// Declares a custom control with a custom element name.
        /// </summary>
        /// <param name="uxmlName">Provides a custom name for the element.</param>
        public UxmlElementAttribute(string uxmlName)
        {
            name = uxmlName;
        }
    }

    /// <summary>
    /// Declares that a field or property is associated with a UXML attribute.
    /// </summary>
    /// <remarks>
    /// You can use the UxmlAttribute attribute to declare that a property or field is associated with a UXML attribute.
    /// When an element is marked with the UxmlElement attribute, a corresponding <see cref="UxmlSerializedData"/> class is
    /// generated in the partial class.
    /// This data class contains a <see cref="SerializeField"/> for each field or property that's marked with the UxmlAttribute attribute. 
    /// When a field or property is associated with a UXML attribute, all of its attributes are transferred over to the serialized version.
    /// This allows for the support of custom property drawers and decorator attributes, such as <see cref="HeaderAttribute"/>, <see cref="TextAreaAttribute"/>,
    /// <see cref="RangeAttribute"/>, <see cref="TooltipAttribute"/>, and so on.
    /// </remarks>
    /// <example>
    /// The following example creates a custom control with custom attributes:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_MyElement.cs"/>
    /// </example>
    /// <example>
    /// Unity objects can also be UxmlAttributes and will contain a reference to the asset file when serialized as UXML.
    /// The following example creates a custom control with custom attributes.
    /// The types of attributes are UXML template and texture:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_CustomElement.cs"/>
    /// </example>
    /// <example>
    /// By default, when resolving the attribute name, the field or property name splits into lowercase words connected by hyphens.
    /// The original uppercase characters in the name are used to denote where the name should be split. For example, if the property name
    /// is @@myIntValue@@, the corresponding attribute name would be @@my-int-value@@.
    /// You can customize the attribute name through the UxmlAttribute name argument. 
    /// The following example creates a custom control with customized attribute names:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_CustomAttributeNameExample.cs"/>
    /// </example>
    /// <example>
    /// Example UXML:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_CustomAttributeNameExample.uxml"/>
    /// </example>
    /// <example>
    /// If you've changed the name of an attribute and want to ensure that UXML files with the previous attribute name can still
    /// be loaded by UI Builder, use the @@obsoleteNames@@ argument.
    /// This argument matches attributes in the UXML to be applied to the attribute during serialization.
    /// UI Builder uses the new name when loading the UXML file.
    /// The following example creates a custom control with custom attributes that uses obsolete attribute names:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_CharacterDetails.cs"/>
    /// </example>
    /// <example>
    /// The following example UXML uses the obsolete names:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_CharacterDetails.uxml"/>
    /// </example>
    /// <example>
    /// When you create each SerializeField, all attributes are copied across.
    /// This allows you to use decorators and custom property drawers on fields in the UI Builder.
    /// The following example uses a custom control with decorators on its attribute fields:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_ExampleText.cs"/>
    /// </example>
    /// <example>
    /// The UI Builder displays the attributes with the decorators:
    ///
    /// 
    ///{img UIB-decorators.png}
    ///
    /// The following example creates a custom control with a custom property drawer:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_MyDrawerExample.cs"/>
    /// </example>
    /// <example>
    /// __Note:__ To access other serialized properties, prepend the name with @@serializedData@@.
    /// For example, to find the @@myColor@@ attribute, use @@serializedData.myColor@@.
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_MyDrawerAttributePropertyDrawer.cs"/>
    /// </example>
    /// <example>
    ///{img UIB-propertydrawer.gif}
    ///
    /// You can use struct or class instances as attributes and even lists of struct or class instances in UXML.However,
    /// they must be convertible to and from a string and you must declare a ::ref::UxmlAttributeConverter to support this conversion.
    /// When using the class in a list, ensure that its string representation does not contain any commas (",") as this character is used by
    /// the list to separate the items.
    /// The following example shows how a class instance can support a property and list:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_MyClassWithData.cs"/>
    /// </example>
    /// <example>
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_MyClassWithDataConverter.cs"/>
    /// </example>
    /// <example>
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlAttribute_MyClassWithDataConverter.uxml"/>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UxmlAttributeAttribute : Attribute
    {
        /// <summary>
        /// Provides a custom UXML name to the attribute.
        /// </summary>
        public string name;

        /// <summary>
        /// Provides support for obsolete UXML attribute names.
        /// </summary>
        public string[] obsoleteNames;

        /// <summary>
        /// Declares that a field or property is associated with a UXML attribute.
        /// </summary>
        public UxmlAttributeAttribute() : this(null, null)
        { }

        /// <summary>
        /// Declares that a field or property is associated with a UXML attribute.
        /// </summary>
        /// <param name="name">The name of the UXML attribute.</param>
        public UxmlAttributeAttribute(string name) : this(name, null)
        { }

        /// <summary>
        /// Declares that a field or property is associated with a UXML attribute.
        /// </summary>
        /// <param name="name">The name of the UXML attribute.</param>
        /// <param name="obsoleteNames">The Obsolete UXML attribute names.</param>
        public UxmlAttributeAttribute(string name, params string[] obsoleteNames)
        {
            this.name = name;
            this.obsoleteNames = obsoleteNames;
        }
    }

    /// <summary>
    /// Provides information about the expected type when applied to a Type field or property that has the <see cref="UxmlAttributeAttribute"/> attribute.
    /// </summary>
    /// <remarks>
    /// When defining a Type field or property with the <see cref="UxmlAttributeAttribute"/> in Unity, you can use the UxmlTypeReference
    /// attribute to specify the base type that the value should inherit from.
    /// This allows you to provide additional information about the expected type of the value and helps Unity ensure that the
    /// correct type is assigned to the attribute.
    /// </remarks>
    /// <example>
    /// The following example creates a custom control and restricts the attribute type to only accept values that are derived from [[VisualElement]]:
    /// <code source="../../../../Modules/UIElements/Tests/UIElementsExamples/Assets/Examples/UxmlTypeReferenceAttribute_TypeRestrictionExample.cs"/>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UxmlTypeReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// The base type that the value inherits from.
        /// </summary>
        public Type baseType;

        /// <summary>
        /// Provides information about the expected type when applied to a Type field or property that has the <see cref="UxmlAttributeAttribute"/> attribute.
        /// </summary>
        public UxmlTypeReferenceAttribute() : this(null)
        { }

        /// <summary>
        /// Provides information about the expected type when applied to a Type field or property that has the <see cref="UxmlAttributeAttribute"/> attribute.
        /// </summary>
        /// <param name="baseType">The base type that the value inherits from.</param>
        public UxmlTypeReferenceAttribute(Type baseType)
        {
            this.baseType = baseType;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    internal class UxmlObjectAttribute : Attribute
    {
        public string name;

        public UxmlObjectAttribute() : this(null)
        { }

        public UxmlObjectAttribute(string uxmlName)
        {
            name = uxmlName;
        }
    }
}
