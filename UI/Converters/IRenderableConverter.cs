using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorSpace.MapData.Interfaces;
using VectorSpace.MapData.MapItems;

namespace VectorSpace.UI.Converters
{
    public class IRenderableConverter : JsonCreateConverter<IRenderable>
    {
        protected override IRenderable Create(Type objectType, JObject jObject)
        {
            if (jObject["Type"] != null)
            {
                switch ((string)jObject["Type"])
                {
                    case "TextureItem":
                        return new TextureItem(
                            (string)jObject["Layer"],
                            (string)jObject["Name"],
                            (string)jObject["TextureName"]
                        );

                    default:
                        throw new InvalidOperationException(string.Format("Type {0} is not supported.", (string)jObject["Type"]));
                }
            }

            throw new InvalidOperationException("No Type property was found.");
        }
    }
}
