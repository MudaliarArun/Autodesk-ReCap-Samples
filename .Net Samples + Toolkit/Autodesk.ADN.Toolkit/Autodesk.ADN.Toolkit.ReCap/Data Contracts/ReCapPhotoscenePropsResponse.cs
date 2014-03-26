using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Autodesk.ADN.Toolkit.ReCap.DataContracts
{
    /////////////////////////////////////////////////////////////////////////////////
    // ReCap Photoscene properties Response
    //
    /////////////////////////////////////////////////////////////////////////////////
    class ReCapPhotoscenePropsResponse : ReCapResponseBase
    {
        [JsonProperty(PropertyName = "Photoscenes")]
        [JsonConverter(typeof(ReCapPhotosceneContainerConverter))]
        public ReCapPhotoscene Photoscene
        {
            get;
            private set;
        }

        [JsonConstructor]
        public ReCapPhotoscenePropsResponse(
           ReCapPhotoscene photoscene)
        {
            Photoscene = photoscene;
        }

        public ReCapPhotoscenePropsResponse()
        {

        }

        public ReCapPhotosceneResponse ToPhotosceneResponse()
        {
            return new ReCapPhotosceneResponse(
                Photoscene,
                Usage,
                Resource,
                Error);
        }
    }
}
