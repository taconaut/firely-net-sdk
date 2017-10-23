﻿/* 
 * Copyright (c) 2017, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://github.com/ewoutkramer/fhir-net-api/blob/master/LICENSE
 */

using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;

namespace Hl7.Fhir.Specification.Source
{
    /// <summary>
    /// Delegate for harvesting an <see cref="ArtifactSummary"/> from the current resource of an
    /// <see cref="INavigatorStream"/>, independent of the underlying serialization format.
    /// </summary>
    /// <param name="origin">The original location of the underlying resource (container).</param>
    /// <param name="nav">Reference to an <see cref="INavigatorStream"/> instance.</param>
    public delegate ArtifactSummary ArtifactSummaryHarvester(string origin, INavigatorStream nav);

    /// <summary>Provides a default implementation for the <see cref="ArtifactSummaryHarvester"/> delegate.</summary>
    public static class DefaultArtifactSummaryHarvester
    {
        /// <summary>
        /// Extension method to harvest summary information from a set of resources via <see cref="INavigatorStream"/>,
        /// independent of the underlying resource serialization format.
        /// </summary>
        /// <param name="harvester">A function that generates an <see cref="ArtifactSummary"/> from the current resource of the specified <see cref="INavigatorStream"/> instance.</param>
        /// <param name="factory">Reference to a <see cref="NavigatorStreamFactory"/>.</param>
        /// <param name="origin">The original location of the underlying resource (container).</param>
        /// <returns>A sequence of <see cref="ArtifactSummary"/> instances for each of the target resources.</returns>
        public static List<ArtifactSummary> HarvestAll(this ArtifactSummaryHarvester harvester, NavigatorStreamFactory factory, string origin)
        {
            // Note: HarvestAll may throw while enumerating, must handle exceptions here

            // [WMR 20171023] In case of error, return completed summaries and error info
            var result = new List<ArtifactSummary>();

            INavigatorStream navStream = null;
            try
            {
                navStream = factory(origin);
                foreach (var summary in harvester.HarvestAll(origin, navStream))
                {
                    result.Add(summary);
                }
            }
            catch (Exception ex)
            {
                result.Add(ArtifactSummary.FromException(origin, ex));
            }
            finally
            {
                navStream?.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Extension method to harvest summary information from a set of resources via the specified
        /// <see cref="INavigatorStream"/>, independent of the underlying resource serialization format.
        /// </summary>
        /// <param name="harvester">A function that generates an <see cref="ArtifactSummary"/> from the current resource of the specified <see cref="INavigatorStream"/> instance.</param>
        /// <param name="origin">The original location of the underlying resource (container).</param>
        /// <param name="stream">A sequence of <see cref="IElementNavigator"/> instances for a set of resources.</param>
        /// <returns>A sequence of <see cref="ArtifactSummary"/> instances for each of the target resources.</returns>
        public static IEnumerable<ArtifactSummary> HarvestAll(this ArtifactSummaryHarvester harvester, string origin, INavigatorStream stream)
        {
            if (stream != null)
            {
                // Note: MoveNext may throw, but yield is not allowed within try/catch
                while (stream.MoveNext())
                {
                    var summary = harvester(origin, stream);
                    // Skip invalid input (no summary)
                    if (summary != null)
                    {
                        yield return summary;
                    }
                }
            }
        }

        /// <summary>
        /// Harvest summary information from the current resource of the specified
        /// <see cref="INavigatorStream"/>, independent of the actual resource serialization format.
        /// </summary>
        /// <param name="origin">The original location of the underlying resource (container).</param>
        /// <param name="stream">An <see cref="INavigatorStream"/> instance that is positioned on a current entry.</param>
        /// <returns>A new <see cref="ArtifactSummary"/> record.</returns>
        public static ArtifactSummary Harvest(string origin, INavigatorStream stream)
        {
            // Dynamically create subtypes depending on resource type

            // INavigatorStream.Current property always returns a new navigator instance
            // => cache and reuse the returned instance
            var current = stream.Current;

            // Current returns null for invalid input
            if (current != null)
            {
                try
                {
                    // Note: each call to INavigatorStream.Current returns a new instance
                    // In order to perform an efficient forward-only scan, we only call
                    // .Current once and inject the created navigator into the ctors.

                    var rawType = current.Type;
                    var resType = EnumUtility.ParseLiteral<ResourceType>(rawType);

                    switch (resType)
                    {
                        case ResourceType.ConceptMap:
                            return new ConceptMapSummary(origin, stream, current);
                        case ResourceType.NamingSystem:
                            return new NamingSystemSummary(origin, stream, current);
                        case ResourceType.ValueSet:
                            return new ValueSetSummary(origin, stream, current);
                        case ResourceType.StructureDefinition:
                            return new StructureDefinitionSummary(origin, stream, current);
                        default:
                            if (ModelInfo.IsConformanceResource(rawType))
                            {
                                return new ConformanceResourceSummary(origin, stream, current);
                            };
                            return new ArtifactSummary(origin, stream, current);
                    }
                }
                // XmlException, JsonException
                catch (Exception ex)
                {
                    // return new ArtifactSummary(origin, stream, current, ex);
                    return ArtifactSummary.FromException(origin, ex);
                }

            }
            // Invalid input
            return null;
        }
    }

}
