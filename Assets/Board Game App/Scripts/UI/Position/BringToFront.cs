using UnityEngine;

namespace UI.Position
{
    class BringToFront : MonoBehaviour
    {
        void OnEnable()
        {
            transform.SetAsLastSibling();  // This is very, very simple.  It takes the Model Panel and changes the
        }                                  // order in the Hierarchy, thus putting it on top of everything else.
    }
}
