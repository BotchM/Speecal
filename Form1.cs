using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using System.Data.Services.Client;
using System.Net;

namespace L_and_write
{
    public partial class SpeechToText : Form
    {
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
        SpeechSynthesizer sythesizer = new SpeechSynthesizer();
        

        public SpeechToText()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnExit.Enabled = false;
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
            btnExit.Enabled = true;

        }

        private void SpeechToText_Load(object sender, EventArgs e)
        {
            MessageBox.Show("What is your name? Enter in the name field.");
            recEngine.SetInputToDefaultAudioDevice();
            recEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recEngine_SpeechRecognized);
            Grammar gr = new Grammar(CreateGrammar());
            recEngine.LoadGrammarAsync(gr);

            sythesizer.Rate = -1;
            sythesizer.GetInstalledVoices();
            sythesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
        }


        void recEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Passing speech by user into the bing search bam bing
            //ANthing said will be searched for before it is computed or calulated or anything
            //txtText.Text = e.Result.Text;
            //Bing(e.Result.Text);

            //The vaariable type for this is RecognitionResult
            RecognitionResult recoResult = e.Result;
            Int32 op1, op2;
            char operation;

            if (recoResult != null)
            {
                if (recoResult.Text.Contains("Search"))
                {
                    string search;
                }
                else if (recoResult.Text == "Intoduce yourself")
                {
                    welcome();
                }
                else if (recoResult.Text == "Botch")
                {
                    Botch();
                }
                else if (recoResult.Text.Contains("exit"))
                {
                    this.Close();
                }
                else if (recoResult.Text == "How are you today, botch. I know we wroked late yesterday!")
                {
                    Botch();
                }
                else if (recoResult.Text.Contains("calculate"))
                {

                    op1 = Convert.ToInt32(recoResult.Semantics["numberf"].Value.ToString());
                    operation = Convert.ToChar(recoResult.Semantics["factorial"].Value.ToString());
                    factorial(op1, operation);

                }
                else if (recoResult.Text.Contains("Hi"))
                {
                    welcome();
                }
                else
                {
                    op1 = Convert.ToInt32(recoResult.Semantics["number1"].Value.ToString());
                    operation = Convert.ToChar(recoResult.Semantics["operator"].Value.ToString());
                    op2 = Convert.ToInt32(recoResult.Semantics["number2"].Value.ToString());
                    //if this is to happen then call the function andf pass the two values and one operator
                    Calculate(op1, operation, op2);
                }


            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            recEngine.RecognizeAsyncStop();
            btnStart.Enabled = true;
        }

        private void add()
        {
            txtText.Text = ("I am here in the add method!");
        }

        private GrammarBuilder CreateGrammar()
        {

            NumberText numbers = new NumberText();


            //The calculator build
            GrammarBuilder[] gb = new GrammarBuilder[5] { null, null, null, null, null };
            gb[0] = new GrammarBuilder(new Choices("exit"));
            gb[1] = new GrammarBuilder();
            gb[2] = new GrammarBuilder();
            gb[3] = new GrammarBuilder();
            //MAking choices for the different things that could be said for calling one array
            Choices man = new Choices();
            man.Add(new string[] { "Botch can you tell the class how much is", "What is" });
            gb[3].Append(man); //to pass the speech said
            string[] numberString = { "zero", "one", "two", "three", "four",
                               "five", "six", "seven", "eight", "nine", "ten",
                               "eleven", "twelve", "thirteen", "fourteen", "fifteen", 
                               "sixteen", "seventeen", "eighteen", "nineteen", "twenty"};

            //numbers.ToText();

            //This is where the numbers are assigned to their semantic values
            Choices numberChoices = new Choices();
            for (int i = 0; i < numberString.Length; i++)
            {
                numberChoices.Add(new SemanticResultValue(numberString[i], i));
            }
            //first number appended before the operator
            gb[3].Append(new SemanticResultKey("number1", (GrammarBuilder)numberChoices));
            string[] operatorString = { "plus", "and", "minus", "times", "divided by", "squared", "factorial" };
            Choices operatorChoices = new Choices();
            operatorChoices.Add(new SemanticResultValue("plus", "+"));
            operatorChoices.Add(new SemanticResultValue("and", "+"));
            operatorChoices.Add(new SemanticResultValue("minus", "-"));
            operatorChoices.Add(new SemanticResultValue("times", "*"));
            operatorChoices.Add(new SemanticResultValue("multiplied by", "*"));
            operatorChoices.Add(new SemanticResultValue("divided by", "/"));
            operatorChoices.Add(new SemanticResultValue("to the power of", "^"));
            operatorChoices.Add(new SemanticResultValue("squared", "X^x"));
            //operatorChoices.Add(new SemanticResultValue("factorial", "!"));

            //the factorial grammer
            Choices factorialSpeech = new Choices();
            factorialSpeech.Add(new string[] { "calculate" });
            gb[2].Append(factorialSpeech);
            Choices factorial = new Choices();
            factorial.Add(new SemanticResultValue("factorial", "!"));
            gb[2].Append(new SemanticResultKey("numberf", (GrammarBuilder)numberChoices));


            //The intro build// i had to create a new instance for the new subscript match it with a new chice which is intro
            gb[4] = new GrammarBuilder();
            Choices intro = new Choices();
            intro.Add(new string[] { "Intoduce yourself", "Botch", "How are you today, botch. I know we wroked late yesterday!", "Hi" });



            //This is how all of the values are returned through the array. 
            //our error for today is here under this damn line !!!!!!!!!!!!!!!!! irony factorial uses the same symb9ol
            gb[2].Append(new SemanticResultKey("factorial", (GrammarBuilder)factorial));
            gb[3].Append(new SemanticResultKey("operator", (GrammarBuilder)operatorChoices));
            //second number appended after the operator
            gb[3].Append(new SemanticResultKey("number2", (GrammarBuilder)numberChoices));
            gb[4].Append(intro);



            //The returning of the whole array
            Choices choices = new Choices(gb);
            return new GrammarBuilder(choices);
            
        }

        private void welcome()
        {
            string name;
            name = txtFirstName.Text;

            //Working in on how to reply to trrgit
            //recEngine.SpeechRecognized re = new EventHandler<SpeechRecognizedEventArgs>();

            sythesizer.Speak("Hi, " + name + "    " + "How are you today");
            sythesizer.Speak("I will be your manly reliable calculator" );
            sythesizer.Speak("It is nice to meet you my name is");
            sythesizer.Speak("trrgit");

            ////A funnt AI
            //sythesizer.SpeakAsync("Emmmmmmmmmmmmmmm, i just woke up........ ehhhhhhhhhh let me clear my throut. Oh Hi class. This is the first time i have seen anyone besides Mahmoud. Let me introduce myself. Will. I am a calculator, my name is Botch i was created by this guy right here. We have a mutual friendship. Our goal, yes i am a real person... Do not laugh at me. Anyways, our goal for mahmoud and i, is to make a fully talking calcuator that would be used, by virtually anyone. Imagine you are doing any calculation and you get stuck. So all you do is call out the expression, then you hear back and answer.. Okay I will stop rambling. I am speaking too fast, i know, I am, still in my early stages of development. Thank you. Start Clapping please. I will leave it, for Mahmoud to finish.");
        }


        private void Calculate(int op1, char operation, int op2)
        {
            double result = 0;
            String prompt;
            String operationStr = "";
            int numberInt = op1;
            int factorial = numberInt;

            
            if (operation == '/' && op2 == 0)
            {
                prompt = op1.ToString() + " divided by zero is undefined. You cannot divide by zero.";
                Console.WriteLine("{0} {1} {2} is undefined. You cannot divide by zero.", op1, operation, op2);
                sythesizer.Speak(prompt);
            }
            else
            {
                switch (operation)
                {
                    case '+':
                        result = op1 + op2;
                        operationStr = " plus ";
                        break;
                    case '-':
                        result = op1 - op2;
                        operationStr = " minus ";
                        break;
                    case '*':
                        result = op1 * op2;
                        operationStr = " times ";
                        break;
                    case '/':
                        result = op1 / op2;
                        operationStr = " divided by ";
                        break;
                    case '^':
                        result = Math.Pow(op1, op2);
                        operationStr = " to the power of ";
                        break;
                    case '!':
                        for (int i = 1; i < numberInt; i++)
                        {
                            factorial = factorial * i;
                        }
                        operationStr = " factorial ";
                        break;
                    //case 'x^':

                    //    break;
                }
                Console.WriteLine("{0} {1} {2} = {3}", op1, operation, op2, result);
                if (operation == '+' || operation == '-' || operation == '*' || operation == '/' || operation == '^' || operation == '!')
                {
                    prompt = op1.ToString() + operationStr + op2.ToString() + " is " + result.ToString();
                    sythesizer.Speak(prompt);

                    prompt = op1.ToString() + operationStr + op2.ToString() + " = " + result.ToString();
                    txtText.Text = prompt;

                    if (operation == '!')
                    {
                        operationStr = " factorial";
                        prompt = op1.ToString() + operationStr + " is " + factorial.ToString();
                        sythesizer.Speak(prompt);

                        prompt = op1.ToString() + operationStr + " = " + factorial.ToString();
                        txtText.Text = prompt;
                    }
                }
            }
        }
        //factorial method
        private void factorial(int op1, char operation)
        {
            String prompt;
            String operationStr = "";
            int numberInt = op1;
            int factorial = numberInt;


            for (int i = 1; i < numberInt; i++)
            {
                factorial = factorial * i;
            }

            operationStr = " factorial";
            prompt = op1.ToString() + operationStr + " is " + factorial.ToString();
            sythesizer.Speak(prompt);

            prompt = op1.ToString() + operationStr + " = " + factorial.ToString();
            txtText.Text = prompt;
            
        }

        private void Botch()
        {
            string prompt;
            string weather;
            string time = DateTime.Now.ToString("hh:mm");
            prompt = "Good morning Mahmoud. I am being polite to you. Although you cussed at me yesterday when you were trying to fix me. But i forgive you";
            sythesizer.Speak(prompt);
            System.Threading.Thread.Sleep(2000);
            prompt = "Alright Mahmoud. Let us get to business";
            sythesizer.Speak(prompt);

        }

        public static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }

/*
        // Replace this value with your account key.
        private const string AccountKey = "kYX2H2B6HhvXfsUq9nl+qMLpi2JaXIrnNa/yLo/Qz6c";

        private void Bing(string args)
        {
            try
            {
                MakeRequest(args);
            }
            catch (Exception ex)
            {
                string innerMessage =
                    (ex.InnerException != null) ?
                    ex.InnerException.Message : String.Empty;
                Console.WriteLine("{0}\n{1}", ex.Message, innerMessage);
            }
        }
        
        private void MakeRequest(string query)
        {
            // This is the query expression.
            
            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // The market to use.
            string market = "en-us";
            
            // The composite operations to use.
            string operations = "web+image+video";

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 5 results (per service operation).
            var compositeQuery =
                bingContainer.Composite(operations, query, null, null, market,
                                        null, null, null, null, null,
                                        null, null, null, null, null);
            compositeQuery = compositeQuery.AddQueryOption("$top", 5);

            // Run the query and display the results.
            var compositeResults = compositeQuery.Execute();

            // Object compositeResults is IEnumerable<ExpandableSearchResult>
            // but there's only one ExpandableSearchResult object in the collection.
            foreach (var cResult in compositeResults)
            {
                // Display web results.
                foreach (var result in cResult.Web)
                {
                    listResults.Items.Add("{0}\n\t{1}" + result.Title + result.Url);
                }
                // Display Image results.
                foreach (var result in cResult.Image)
                {
                    listResults.Items.Add("{0}\n\t{1}" + result.Title + result.MediaUrl);
                }
                // Display video results.
                foreach (var result in cResult.Video)
                {
                    int runTime = (result.RunTime != null) ? (int)result.RunTime : 0;
                    int totalSecs = runTime / 1000;
                    int mins = totalSecs / 60;
                    int secs = totalSecs % 60;
                    listResults.Items.Add("{0} ({1:d2}:{2:d2})\n\t{3}" + result.Title + mins + secs + result.MediaUrl);
                }
                // This query used "web+image+video" - If you used other
                // service operations, you could iterate through them also.
            }
        }
 */
    }
}









