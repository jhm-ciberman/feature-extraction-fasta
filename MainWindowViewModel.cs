using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FeatureExtraction.WordExtractor;

namespace FeatureExtraction
{
    class MainWindowViewModel : ObservableObject
    {
        private string text;
        public string Text
        { 
            get => this.text;
            set
            {
                // If the text changed in the UI, then extract all features
                if (this.SetProperty(ref this.text, value))
                {
                    this.ExtractFeaturesFromText();
                }
            }
        }


        private IEnumerable<KeyValuePair<string, int>> bagOfWordsResults;
        public IEnumerable<KeyValuePair<string, int>> BagOfWordsResults 
        { 
            get => this.bagOfWordsResults; 
            protected set => this.SetProperty(ref this.bagOfWordsResults, value); 
        }


        private IEnumerable<Feature> tfidfResults;
        public IEnumerable<Feature> TfidfResults 
        { 
            get => this.tfidfResults;
            protected set => this.SetProperty(ref this.tfidfResults, value); 
        }

        public IRelayCommand LoadDonQuijoteCommand { get; }


        private WordExtractor extractor;
        public MainWindowViewModel()
        {
            this.extractor = new WordExtractor();
            this.LoadDonQuijoteCommand = new RelayCommand(this.LoadDonQuijote);
        }

        private void LoadDonQuijote()
        {
            this.Text = ExampleText.DonQuijote;
        }

        private void ExtractFeaturesFromText()
        {

            this.TfidfResults = this.extractor.GetTermFrecuencyTimesInverseDocumentFrecuency(this.Text)
                .OrderByDescending(feature => feature.Weight);

            this.BagOfWordsResults = this.extractor.GetTermFrecuency(this.Text)
                .OrderByDescending(kv => kv.Value);
        }
    }
}
