// <auto-generated/>
// Contents of: hl7.fhir.r5.core version: 5.0.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Validation;

/*
  Copyright (c) 2011+, HL7, Inc.
  All rights reserved.
  
  Redistribution and use in source and binary forms, with or without modification, 
  are permitted provided that the following conditions are met:
  
   * Redistributions of source code must retain the above copyright notice, this 
     list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above copyright notice, 
     this list of conditions and the following disclaimer in the documentation 
     and/or other materials provided with the distribution.
   * Neither the name of HL7 nor the names of its contributors may be used to 
     endorse or promote products derived from this software without specific 
     prior written permission.
  
  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
  POSSIBILITY OF SUCH DAMAGE.
  
*/

namespace Hl7.Fhir.Model
{
  /// <summary>
  /// Pure binary content defined by a format other than FHIR
  /// </summary>
  [Serializable]
  [DataContract]
  [FhirType("Binary","http://hl7.org/fhir/StructureDefinition/Binary", IsResource=true)]
  public partial class Binary : Hl7.Fhir.Model.Resource
  {
    /// <summary>
    /// FHIR Type Name
    /// </summary>
    public override string TypeName { get { return "Binary"; } }

    /// <summary>
    /// MimeType of the binary content
    /// </summary>
    [FhirElement("contentType", InSummary=true, Order=50)]
    [Binding("MimeType")]
    [Cardinality(Min=1,Max=1)]
    [DataMember]
    public Hl7.Fhir.Model.Code ContentTypeElement
    {
      get { return _ContentTypeElement; }
      set { _ContentTypeElement = value; OnPropertyChanged("ContentTypeElement"); }
    }

    private Hl7.Fhir.Model.Code _ContentTypeElement;

    /// <summary>
    /// MimeType of the binary content
    /// </summary>
    /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
    [IgnoreDataMember]
    public string ContentType
    {
      get { return ContentTypeElement != null ? ContentTypeElement.Value : null; }
      set
      {
        if (value == null)
          ContentTypeElement = null;
        else
          ContentTypeElement = new Hl7.Fhir.Model.Code(value);
        OnPropertyChanged("ContentType");
      }
    }

    /// <summary>
    /// Identifies another resource to use as proxy when enforcing access control
    /// </summary>
    [FhirElement("securityContext", InSummary=true, Order=60)]
    [CLSCompliant(false)]
    [References("Resource")]
    [DataMember]
    public Hl7.Fhir.Model.ResourceReference SecurityContext
    {
      get { return _SecurityContext; }
      set { _SecurityContext = value; OnPropertyChanged("SecurityContext"); }
    }

    private Hl7.Fhir.Model.ResourceReference _SecurityContext;

    /// <summary>
    /// The actual content. Note: Element was introduced in R4, do not use when working with older releases.
    /// </summary>
    [FhirElement("data", Order=70, Since=FhirRelease.R4)]
    [DataMember]
    public Hl7.Fhir.Model.Base64Binary DataElement
    {
      get { return _DataElement; }
      set { _DataElement = value; OnPropertyChanged("DataElement"); }
    }

    private Hl7.Fhir.Model.Base64Binary _DataElement;

    /// <summary>
    /// The actual content
    /// </summary>
    /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
    [IgnoreDataMember]
    public byte[] Data
    {
      get { return DataElement != null ? DataElement.Value : null; }
      set
      {
        if (value == null)
          DataElement = null;
        else
          DataElement = new Hl7.Fhir.Model.Base64Binary(value);
        OnPropertyChanged("Data");
      }
    }

    /// <summary>
    /// The actual content. Note: Element is replaced by 'Binary.data' since R4. Do not use this element 'content' with R4 and newer releases.
    /// </summary>
    [FhirElement("content", Order=70)]
    [NotMapped(Since=FhirRelease.R4)]
    [Cardinality(Min=1,Max=1)]
    [DataMember]
    public Hl7.Fhir.Model.Base64Binary ContentElement
    {
      get { return _ContentElement; }
      set { _ContentElement = value; OnPropertyChanged("ContentElement"); }
    }

    private Hl7.Fhir.Model.Base64Binary _ContentElement;

    /// <summary>
    /// The actual content
    /// </summary>
    /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
    [IgnoreDataMember]
    public byte[] Content
    {
      get { return ContentElement != null ? ContentElement.Value : null; }
      set
      {
        if (value == null)
          ContentElement = null;
        else
          ContentElement = new Hl7.Fhir.Model.Base64Binary(value);
        OnPropertyChanged("Content");
      }
    }

    public override IDeepCopyable CopyTo(IDeepCopyable other)
    {
      var dest = other as Binary;

      if (dest == null)
      {
        throw new ArgumentException("Can only copy to an object of the same type", "other");
      }

      base.CopyTo(dest);
      if(ContentTypeElement != null) dest.ContentTypeElement = (Hl7.Fhir.Model.Code)ContentTypeElement.DeepCopy();
      if(SecurityContext != null) dest.SecurityContext = (Hl7.Fhir.Model.ResourceReference)SecurityContext.DeepCopy();
      if(DataElement != null) dest.DataElement = (Hl7.Fhir.Model.Base64Binary)DataElement.DeepCopy();
      if(ContentElement != null) dest.ContentElement = (Hl7.Fhir.Model.Base64Binary)ContentElement.DeepCopy();
      return dest;
    }

    public override IDeepCopyable DeepCopy()
    {
      return CopyTo(new Binary());
    }

    ///<inheritdoc />
    public override bool Matches(IDeepComparable other)
    {
      var otherT = other as Binary;
      if(otherT == null) return false;

      if(!base.Matches(otherT)) return false;
      if( !DeepComparable.Matches(ContentTypeElement, otherT.ContentTypeElement)) return false;
      if( !DeepComparable.Matches(SecurityContext, otherT.SecurityContext)) return false;
      if( !DeepComparable.Matches(DataElement, otherT.DataElement)) return false;
      if( !DeepComparable.Matches(ContentElement, otherT.ContentElement)) return false;

      return true;
    }

    public override bool IsExactly(IDeepComparable other)
    {
      var otherT = other as Binary;
      if(otherT == null) return false;

      if(!base.IsExactly(otherT)) return false;
      if( !DeepComparable.IsExactly(ContentTypeElement, otherT.ContentTypeElement)) return false;
      if( !DeepComparable.IsExactly(SecurityContext, otherT.SecurityContext)) return false;
      if( !DeepComparable.IsExactly(DataElement, otherT.DataElement)) return false;
      if( !DeepComparable.IsExactly(ContentElement, otherT.ContentElement)) return false;

      return true;
    }

    [IgnoreDataMember]
    public override IEnumerable<Base> Children
    {
      get
      {
        foreach (var item in base.Children) yield return item;
        if (ContentTypeElement != null) yield return ContentTypeElement;
        if (SecurityContext != null) yield return SecurityContext;
        if (DataElement != null) yield return DataElement;
        if (ContentElement != null) yield return ContentElement;
      }
    }

    [IgnoreDataMember]
    public override IEnumerable<ElementValue> NamedChildren
    {
      get
      {
        foreach (var item in base.NamedChildren) yield return item;
        if (ContentTypeElement != null) yield return new ElementValue("contentType", ContentTypeElement);
        if (SecurityContext != null) yield return new ElementValue("securityContext", SecurityContext);
        if (DataElement != null) yield return new ElementValue("data", DataElement);
        if (ContentElement != null) yield return new ElementValue("content", ContentElement);
      }
    }

    protected override bool TryGetValue(string key, out object value)
    {
      switch (key)
      {
        case "contentType":
          value = ContentTypeElement;
          return ContentTypeElement is not null;
        case "securityContext":
          value = SecurityContext;
          return SecurityContext is not null;
        case "data":
          value = DataElement;
          return DataElement is not null;
        case "content":
          value = ContentElement;
          return ContentElement is not null;
        default:
          return base.TryGetValue(key, out value);
      }

    }

    protected override IEnumerable<KeyValuePair<string, object>> GetElementPairs()
    {
      foreach (var kvp in base.GetElementPairs()) yield return kvp;
      if (ContentTypeElement is not null) yield return new KeyValuePair<string,object>("contentType",ContentTypeElement);
      if (SecurityContext is not null) yield return new KeyValuePair<string,object>("securityContext",SecurityContext);
      if (DataElement is not null) yield return new KeyValuePair<string,object>("data",DataElement);
      if (ContentElement is not null) yield return new KeyValuePair<string,object>("content",ContentElement);
    }

  }

}

// end of file
