using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

namespace Padoru.Localization
{
	public interface ILocalizationManager
	{
		Languages CurrentLanguage { get; }
		
		event Action<Languages> OnLanguageChanged;

		void SetLanguage(Languages language);

		Task LoadFile(Languages language, string fileUri);

		void AddFile(Languages language, LocalizationFile file);

		string GetLocalizedText(string entryName);

		bool HasLocalizedText(string entryName);

		bool TryGetLocalizedText(string entryName, out string localizedText);

		void SetLanguageFontOverrides(TMP_FontAsset baseFont, TMP_FontAsset baseTitleFont, Dictionary<Languages, TMP_FontAsset> overrides, Dictionary<Languages, TMP_FontAsset> titleOverrides);

		TMP_FontAsset GetCurrentLanguageFont(bool isTitle);
	}
}
