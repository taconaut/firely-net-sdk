﻿/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://github.com/FirelyTeam/firely-net-sdk/blob/master/LICENSE
 */


using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Numerics;

namespace Hl7.Fhir.Serialization
{
    internal class FhirJsonBuilder : IExceptionSource
    {
        internal FhirJsonBuilder(FhirJsonSerializationSettings settings = null)
        {
            _settings = settings?.Clone() ?? new FhirJsonSerializationSettings();
        }

        private FhirJsonSerializationSettings _settings;
        private bool _roundtripMode = true;

        public ExceptionNotificationHandler ExceptionHandler { get; set; }

        public JObject Build(ITypedElement source) => buildInternal(source);

        public JObject Build(ISourceNode source)
        {
            bool hasJsonSource = source.Annotation<FhirJsonNode>() != null;

            // We can only work with an untyped source if we're doing a roundtrip,
            // so we have all serialization details available.
            if (hasJsonSource)
            {
                _roundtripMode = true;          // will allow unknown elements to be processed
#pragma warning disable 612,618
                return buildInternal(source.ToTypedElement());
#pragma warning restore 612,618
            }
            else
            {
                throw Error.NotSupported($"The {nameof(FhirJsonBuilder)} will only work correctly on an untyped " +
                    $"source if the source is a {nameof(FhirJsonNode)}.");
            }
        }

        private JObject buildInternal(ITypedElement source)
        {
            var dest = new JObject();

            if (source is IExceptionSource)
            {
                using (source.Catch((o, a) => ExceptionHandler.NotifyOrThrow(o, a)))
                {
                    addChildren(source, dest);
                }
            }
            else
                addChildren(source, dest);

            return dest;
        }


        // These are the "primitive" FHIR instance types that possibly need a separate element/_element
        // serialization in json.
        private static readonly string[] primitiveTypes =
        {
            "boolean",
             "integer",
             "integer64",
             "unsignedInt",
             "positiveInt",
             "time",
             "date",
             "instant",
             "dateTime",
             "decimal",
             "string",
             "code",
             "id",
             "uri",
             "oid",
             "uuid",
             "canonical",
             "url",
             "markdown",
             "base64Binary",
             "xhtml"
        };


        private (JToken first, JObject second) buildNode(ITypedElement node)
        {
            var details = node.GetJsonSerializationDetails();
            object value = node.Definition != null ? node.Value : details?.OriginalValue ?? node.Value;
            var objectInShadow = node.InstanceType != null ? primitiveTypes.Contains(node.InstanceType) : details?.UsesShadow ?? false;

            JToken first = value != null ? buildValue(value, node.InstanceType) : null;
            JObject second = buildChildren(node);

            // If this is a complex type with a value (should not occur)
            // serialize it like a primitive, otherwise, the first member
            // is just the normal content of the complex type (the children)
            if (!objectInShadow && first == null)
            {
                if (first == null)
                {
                    first = second;
                    second = null;
                }
            }

            return (first, second);

            JObject buildChildren(ITypedElement n)
            {
                var objectWithChildren = new JObject();
                addChildren(n, objectWithChildren);

                return objectWithChildren.Count == 0 ? null : objectWithChildren;
            }
        }

        internal bool MustSerializeMember(ITypedElement source, out IElementDefinitionSummary info)
        {
            info = source.Definition;

            if (info == null && !_roundtripMode)
            {
                var message = $"Element '{source.Location}' is missing type information.";

                if (_settings.IgnoreUnknownElements)
                {
                    ExceptionHandler.NotifyOrThrow(source, ExceptionNotification.Warning(
                        new MissingTypeInformationException(message)));
                }
                else
                {
                    ExceptionHandler.NotifyOrThrow(source, ExceptionNotification.Error(
                        new MissingTypeInformationException(message)));
                }

                return false;
            }

            return true;
        }

        private void addChildren(ITypedElement node, JObject parent)
        {
            var resourceTypeIndicator = node.Annotation<IResourceTypeSupplier>()?.ResourceType;
            var isResource = node.Definition?.IsResource ?? resourceTypeIndicator != null;
            var containedResourceType = isResource ? (node.InstanceType ?? resourceTypeIndicator) : null;
            if (containedResourceType != null)
                parent.AddFirst(new JProperty(JsonSerializationDetails.RESOURCETYPE_MEMBER_NAME, containedResourceType));

            foreach (var nameGroup in node.Children().GroupBy(n => n.Name))
            {
                var members = nameGroup.ToList();

                // serialization info should be the same for each element in an
                // array - but do not explicitly check that
                if (!MustSerializeMember(members[0], out var generalInfo)) break;
                bool hasTypeInfo = generalInfo != null;

                // If we have type information, we know whather we need an array.
                // failing that, check whether this is a roundtrip and we have the information
                // about arrays in the serialization deails. Failing that, assume the default:
                // for unknown properties is to use an array - safest bet.
                var generalJsonDetails = members[0].GetJsonSerializationDetails();
                var hasIndex = generalJsonDetails?.ArrayIndex != null;
                var needsArray = generalInfo?.IsCollection ?? hasIndex;

                var children = members.Select(m => buildNode(m))
                            .Where(c => !(c.first == null && c.second == null)).ToList();

                // Don't add empty nodes to the parent
                if (!children.Any()) continue;

                var needsMainProperty = children.Any(c => c.first != null);
                var needsShadowProperty = children.Any(c => c.second != null);
                var propertyName = generalInfo?.IsChoiceElement == true ?
                        members[0].Name + members[0].InstanceType.Capitalize() : members[0].Name;

                if (needsMainProperty)
                    parent.Add(new JProperty(propertyName,
                        needsArray ? new JArray(children.Select(c => c.first ?? JValue.CreateNull())) : children[0].first));

                if (needsShadowProperty)
                    parent.Add(new JProperty("_" + propertyName,
                        needsArray ? new JArray(children.Select(c => (JToken)c.second ?? JValue.CreateNull())) : (JToken)children[0].second));
            }
        }

        private JValue buildValue(object value, string requiredType = null) => value switch
        {
            bool or decimal or Int32 or Int16 or ulong or double or BigInteger or float => new JValue(value),
            string s => new JValue(s.Trim()),
            long l when requiredType is "integer" or "unsignedInt" or "positiveInt" => new JValue(l),
            _ => new JValue(PrimitiveTypeConverter.ConvertTo<string>(value)),
        };
    }
}
