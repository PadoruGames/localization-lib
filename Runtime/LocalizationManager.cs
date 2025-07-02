using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Debug = Padoru.Diagnostics.Debug;

namespace Padoru.Localization
{
	public class LocalizationManager : ILocalizationManager
	{
		private readonly ILocalizationFilesLoader filesLoader;
		private readonly Dictionary<Languages, LocalizationFile> files = new();
		
		private TMP_FontAsset baseFont;
		private TMP_FontAsset baseTitleFont;
		private Dictionary<Languages, TMP_FontAsset> fontOverrides = new();
		private Dictionary<Languages, TMP_FontAsset> titleFontOverrides = new();

		public Languages CurrentLanguage { get; private set; }
		
		public event Action<Languages> OnLanguageChanged;

		public LocalizationManager(ILocalizationFilesLoader filesLoader, Languages startingLanguage)
		{
			Debug.Log($"Initialized on {startingLanguage}", Constants.LOCALIZATION_LOG_CHANNEL);

			CurrentLanguage = startingLanguage;

			this.filesLoader = filesLoader;
		}

		public async Task LoadFile(Languages language, string fileUri)
		{
			var file = await filesLoader.LoadFile(fileUri);
			
			Debug.Log($"Localization file loaded {fileUri} for language {language}", Constants.LOCALIZATION_LOG_CHANNEL);

			AddFile(language, file);
		}

		public void AddFile(Languages language, LocalizationFile file)
		{
			if (file == null || file.entries == null || file.entries.Count <= 0)
			{
				return;
			}

			if (!files.ContainsKey(language))
			{
				files.Add(language, new LocalizationFile()
				{
					entries = new Dictionary<string, string>()
				});
			}

			foreach (var entry in file.entries)
			{
				files[language].entries.TryAdd(entry.Key, entry.Value);
			}
		}

		public string GetLocalizedText(string entryName)
		{
			var file = GetFile(CurrentLanguage);
			
			if(!file.entries.ContainsKey(entryName))
			{
				throw new Exception($"The file for {CurrentLanguage} does not contain an entry for {entryName}");
			}

			return file.entries[entryName];
		}

		public bool HasLocalizedText(string entryName)
		{
			if(TryGetFile(CurrentLanguage, out var file))
			{
				return file.entries.ContainsKey(entryName);
			}

			return false;
		}

		public bool TryGetLocalizedText(string entryName, out string localizedText)
		{
			if(TryGetFile(CurrentLanguage, out var file))
			{
				return file.entries.TryGetValue(entryName, out localizedText);
			}

			localizedText = Constants.COULD_NOT_LOCALIZE_STRING;
			return false;
		}

		public void SetLanguageFontOverrides(TMP_FontAsset baseFont, TMP_FontAsset baseTitleFont, Dictionary<Languages, TMP_FontAsset> overrides, Dictionary<Languages, TMP_FontAsset> titleOverrides)
		{
			this.baseFont = baseFont;
			this.baseTitleFont = baseTitleFont;
			this.fontOverrides = overrides;
			this.titleFontOverrides = titleOverrides;
		}

		public TMP_FontAsset GetCurrentLanguageFont(bool isTitle)
		{
			if (isTitle)
			{
				return titleFontOverrides.GetValueOrDefault(CurrentLanguage, baseTitleFont);
			}
			
			return fontOverrides.GetValueOrDefault(CurrentLanguage, baseFont);
		}

		public void SetLanguage(Languages language)
		{
			CurrentLanguage = language;

			Debug.Log($"Language changed to {language}", Constants.LOCALIZATION_LOG_CHANNEL);

			OnLanguageChanged?.Invoke(language);
		}

		private LocalizationFile GetFile(Languages language)
		{
			if (!files.ContainsKey(language))
			{
				throw new Exception($"No localization file loaded for language: {language}");
			}

			return files[language];
		}

		private bool TryGetFile(Languages language, out LocalizationFile file)
		{
			return files.TryGetValue(language, out file);
		}
	}
}
