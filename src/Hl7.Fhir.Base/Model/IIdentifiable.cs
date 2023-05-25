using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hl7.Fhir.Base.Model;

public interface IIdentifiable
{
    List<Identifier> Identifier { get; set; }
}
