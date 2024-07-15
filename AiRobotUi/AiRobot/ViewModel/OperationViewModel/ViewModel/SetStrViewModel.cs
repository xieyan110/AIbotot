using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Nodify;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Markup;
using System.Collections;

namespace Aibot
{
    public class SetStrViewModel : OperationViewModel
    {
        public SetStrViewModel()
        {
            SetStrCommand = new RequeryCommand(() =>
            {
                if (Output.Count > 0)
                {
                    Output[0].Value = SetStr;
                    try
                    {
                        JToken.Parse(SetStr);
                        IsJson = true;
                    }
                    catch (JsonReaderException)
                    {
                        IsJson = false;
                    }
                }

            });

            //if (string.IsNullOrWhiteSpace(SetStr))
            //{
            //    value.ForEach(x =>
            //    {
            //        if (x.PropertyName == "Str")
            //        {
            //            if (!string.IsNullOrWhiteSpace(x.Value?.ToString()))
            //                SetStr = x.Value.ToString();
            //        }
            //    });
            //}

        }

        public INodifyCommand SetStrCommand { get; }

        private string? _setStr;
        public string? SetStr
        {
            get => _setStr;
            set
            {
                if (SetProperty(ref _setStr, value))
                {
                    SetStrCommand.Execute(value);

                }
            }
        }

        private bool? _isJson;
        public bool? IsJson
        {
            get => _isJson;
            set => SetProperty(ref _isJson, value);
        }


    }
}
