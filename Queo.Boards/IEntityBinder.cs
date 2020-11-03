using Newtonsoft.Json;

namespace Queo.Boards {
    public interface IJsonEntityBinder {

        object Bind(JsonReader reader);

        object BindList(JsonReader reader);
    }
}