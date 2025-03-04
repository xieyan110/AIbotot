using System;
using System.Linq;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace Aibot
{
    [AibotItem("窗口-微软TTS", ActionType = ActionType.WindowsServer)]
    public class StartSpeak : BaseAibotAction,IAibotAction
    {
        [AibotProperty("文本(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var text = Text.Value?.ToString() ?? "";
            var synhesizer = new SpeechSynthesizer();

            if (synhesizer.State != SynthesizerState.Speaking)
            {
                var voice = synhesizer.GetInstalledVoices()
                                    .Where(x => x.VoiceInfo.Culture.ToString().Contains("zh-CN"))
                                    .FirstOrDefault();
                if (voice == null) return Task.CompletedTask; ;

                // Configure the audio output. 
                synhesizer.SetOutputToDefaultAudioDevice();

                // Set the volume of the TTS voice, and the combined output volume.
                synhesizer.Volume = 60;

                PromptBuilder builder = new PromptBuilder(
                  new System.Globalization.CultureInfo("zh-CN"));
                builder.AppendText(text);

                synhesizer.SelectVoice(voice.VoiceInfo.Name);
                synhesizer.SelectVoiceByHints(voice.VoiceInfo.Gender, voice.VoiceInfo.Age, 1, voice.VoiceInfo.Culture);
                synhesizer.Speak(builder);

            }
            return Task.CompletedTask;
        }
    }
}
