﻿/* 
 * Copyright (c) 2023, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/FirelyTeam/firely-net-sdk/master/LICENSE
 */


#nullable enable

using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;

namespace Hl7.Fhir.Serialization
{
    /// <summary>
    /// Factory methods for creating the default implementation of <see cref="IFhirSerializationEngine"/>, as used by the
    /// FhirClient.
    /// </summary>
    public static class FhirSerializationEngine
    {
        /// <summary>
        /// Create an implementation of <see cref="IFhirSerializationEngine"/> configured whith defaults,
        /// which uses the "old" TypedElement-based parser and serializer.
        /// </summary>
        public static IFhirSerializationEngine ElementModel(ModelInspector inspector, ParserSettings? settings) =>
            new ElementModelSerializationEngine(inspector, settings);

        /// <summary>
        /// Create an implementation of <see cref="IFhirSerializationEngine"/> configured with defaults 
        /// which uses the new Poco-based parser and serializer.
        /// </summary>
        /// <param name="inspector"></param>
        /// <returns></returns>
        public static IFhirSerializationEngine Poco(ModelInspector inspector) =>  new PocoSerializationEngine(inspector);
    }

    /// <summary>
    /// Represents an object that can serialize/deserialize FHIR data from the supported
    /// serialization formats.
    /// </summary>
    public interface IFhirSerializationEngine
    {
        /// <summary>
        /// Deserialize an XML string to a FHIR Resource POCO.
        /// </summary>
        /// <exception cref="DeserializationFailedException">Thrown when the deserializer encountered one or more errors in the FHIR Xml format.</exception>
        public Resource? DeserializeFromXml(string data);

        /// <summary>
        /// Deserialize a Json string to a FHIR Resource POCO.
        /// </summary>
        /// <exception cref="DeserializationFailedException">Thrown when the deserializer encountered one or more errors in the FHIR Json format.</exception>
        public Resource? DeserializeFromJson(string data);

        /// <summary>
        /// Serialize a FHIR Resource POCO into a string of Xml.
        /// </summary>
        public string SerializeToXml(Resource instance);

        /// <summary>
        /// Serialize a FHIR Resource POCO into a string of Json.
        /// </summary>
        public string SerializeToJson(Resource instance);
    }
}

#nullable restore