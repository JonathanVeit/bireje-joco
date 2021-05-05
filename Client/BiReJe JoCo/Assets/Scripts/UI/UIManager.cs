using JoVei.Base;
using System.Collections;

namespace BiReJeJoCo.UI
{
    public class UIManager : InstanceProvider<UIElement>, IInitializable
    {
        public IEnumerator Initialize(object[] parameters)
        {
            DIContainer.RegisterImplementation<UIManager>(this);
            yield return null;
        }

        public void CleanUp() { }
    }
}
