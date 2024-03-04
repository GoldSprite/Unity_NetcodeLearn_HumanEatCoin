using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GoldSprite.AutoListUI
{
    public class AutoListItem : MonoBehaviour, IAutoListItem
    {
        [SerializeField]
        private Text NameTxt;
        public string Name;

        public virtual void UpdateContent()
        {
            NameTxt.text = Name;
        }
    }
}
