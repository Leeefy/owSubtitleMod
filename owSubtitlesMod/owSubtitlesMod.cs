using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace owSubtitlesMod;

public class owSubtitlesMod : ModBehaviour
{
   private Font _hudFont;
   private int _Xpos;
   private int _Ypos; 
   private int _Xsize;
   private int _sensitivity;
   private Transform _player;
   private OWScene _currentScene;
   private AudioSource[] _sources;
   private string _subtitleText;
   private string _subtitleTest = "test1\ntest2";
	public static owSubtitlesMod Instance;

	public void Awake()
	{
		Instance = this;
		// You won't be able to access OWML's mod helper in Awake.
		// So you probably don't want to do anything here.
		// Use Start() instead.
	}

	public void Start()
	{
		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"{nameof(owSubtitlesMod)} loading...", MessageType.Info);
      _hudFont = Resources.Load<Font>(@"fonts/english - latin/SpaceMono-Regular_Dynamic");

		new Harmony("Ki_ii.owSubtitlesMod").PatchAll(Assembly.GetExecutingAssembly());


		// Example of accessing game code.
		OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
		LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
      
	}

   public void OnGUI()
   {
      _subtitleText = "";
      _sources = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      _player = player.transform;
      foreach(AudioSource audiosource in _sources)
      {
         float distance = Vector3.Distance(audiosource.transform.position, _player.position);

         if(audiosource.isPlaying && distance <= (_sensitivity * 1.0f))
         {
            _subtitleText +=  distance.ToString("n2") + "m: " + audiosource.name + "\n";
         }

      }
      _Xpos = ModHelper.Config.GetSettingsValue<int>("X-pos");
      _Ypos = ModHelper.Config.GetSettingsValue<int>("Y-pos");
      _Xsize = ModHelper.Config.GetSettingsValue<int>("Font size");
      _sensitivity = ModHelper.Config.GetSettingsValue<int>("Sensitivity");
      var style = new GUIStyle 
      {
         font = _hudFont,
         fontSize = _Xsize,
         wordWrap = false
      };
      style.normal.textColor = Color.white;
		if (_currentScene != OWScene.SolarSystem) return;
      GUI.Label(new Rect(_Xpos, _Ypos, 1000, 1000), _subtitleText, style);
   }

	public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
	{
      _currentScene = newScene;
		if (newScene != OWScene.SolarSystem) return;
		//ModHelper.Console.WriteLine("Ingame now - Subtitles will now be displayed!", MessageType.Success);
   }
   
}
