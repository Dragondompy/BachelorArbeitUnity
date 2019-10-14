using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BachelorArbeitUnity
{
    public class errorMessageHandler : MonoBehaviour
    {
        public GameObject errorText;
        public string textStart;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setError(List<Edge> edges)
        {
            string add = "";
            foreach (Edge e in edges)
            {
                add += " " + e.getSepNumber();
            }
            errorText.GetComponent<TextMeshProUGUI>().SetText(add);
        }

        public void selfDestroy() {
            Destroy(this.gameObject);
        }
    }
}