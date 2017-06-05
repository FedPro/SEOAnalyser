using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;


namespace SEO_Analyser
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SortExpr"] = "SortbyWord ASC";
            }
        }


        protected void AnalyzeMetaTags(string responseRequest)
        {
            ///meta tags content row
            var MTcontent = HtmlParserHelper.GetMetaTagsContent(responseRequest);
            var words = MTcontent.Split(AnalyzerUtilities.GetDelimitersList(), StringSplitOptions.RemoveEmptyEntries);
            // dictionary to store and count the words found
            Dictionary<string, int> wordsCounter = new Dictionary<string, int>();
            foreach (string s in words)
            {
                string w = s.ToLower();
                wordsCounter = AnalyzerUtilities.AddWordToList(w, wordsCounter, chkMtKey.Checked);
            }
            //Binding data
            gvMetaTags.DataSource = wordsCounter;
            gvMetaTags.DataBind();
            gvMetaTags.Visible = true;
        }

        protected int AnalyzeLinks(string pText)
        {
            StringBuilder sb = new StringBuilder();
            ///reg to extract links
            var regLinks = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.IgnoreCase);
            foreach (Match m in regLinks.Matches(pText))
                sb.Append(m.Value + ";");

            return sb.ToString().Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries).Count();
        }


        protected int AnalyzeText(string InputTxt)
        {
            // dictionary to store and count the words found
            Dictionary<string, int> wordsCounter = new Dictionary<string, int>();
            // initialization         
            var stopW = new string[] { }; ;
            if (chkStopwords.Checked) // Load the stop words only in case we need filter out the text
                stopW = AnalyzerUtilities.GetStopWordsList();        
            // slipt the input string by delimiters
            var Inputwords = InputTxt.Split(AnalyzerUtilities.GetDelimitersList(), StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string s in Inputwords)
            {
                string w = s.ToLower();
                // remove stopwprds in case the filter is checked
                if (chkStopwords.Checked)
                    if (stopW.Contains(w))
                        w = "";

                // Add word to the counter list
                wordsCounter = AnalyzerUtilities.AddWordToList(w, wordsCounter, chkKeys.Checked);
            }
            //Binding data
            ViewState["dicWords"] = wordsCounter;
            BindOrderedData(ViewState["SortExpr"].ToString());
            return wordsCounter.Count();
        }



        protected void inputTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtInput.Text = "";
            if (((System.Web.UI.WebControls.ListControl)sender).SelectedValue == "url")
            { 
                txtInput.TextMode = TextBoxMode.Url;
                chkMtKey.Enabled = true;
                chkMtKey.Checked = true;
            }
            else
            {
                txtInput.TextMode = TextBoxMode.MultiLine;
                chkMtKey.Enabled = false;
                chkMtKey.Checked = false;
            }
        }

        protected void gvData_Sorting(object sender, GridViewSortEventArgs e)
        {
            //get the current state of the sort expression
            string[] State_SortExpressions = ViewState["SortExpr"].ToString().Split(' ');

            // check about which column
            if (State_SortExpressions[0] == e.SortExpression)
            {
                ViewState["SortExpr"] = State_SortExpressions[1] == "ASC" ? e.SortExpression + " " + "DESC" : e.SortExpression + " " + "ASC";

            }
            // in case of another column - sort by ASC
            else
            {
                ViewState["SortExpr"] = e.SortExpression + " " + "ASC";
            }
            BindOrderedData(ViewState["SortExpr"].ToString());
        }

        protected void Totals(string pAction, int totLinks = 0, int totWords = 0)
        {
            switch (pAction)
            {
                case "bind":
                    lblTotWords.Visible = true;
                    lblTotWords.Text = "Total unique words: " + totWords.ToString();
                    if (chkLinks.Checked)
                    {
                        lblTotLinks.Visible = true;
                        lblTotLinks.Text = "Total external links: " + totLinks.ToString();
                    }
                    break;
                case "reset":
                    lblTotWords.Text = "";
                    lblTotLinks.Text = "";
                    lblTotWords.Visible = false;
                    lblTotLinks.Visible = false;
                    break;
            }
        }
        protected void BindOrderedData(string expr)
        {
            if (ViewState["dicWords"] != null)
            {
                // Get the data from ViewState
                Dictionary<string, int> wordsCounter = (Dictionary<string, int>)ViewState["dicWords"];
                var sortedDict = new Dictionary<string, int>();
                switch (expr)
                {
                    case "SortbyWord ASC":
                        sortedDict = wordsCounter.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                        break;
                    case "SortbyWord DESC":
                        sortedDict = wordsCounter.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
                        break;
                    case "SortbyCount ASC":
                        sortedDict = wordsCounter.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        break;
                    case "SortbyCount DESC":
                        sortedDict = wordsCounter.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        break;
                }
                ViewState["dicWords"] = sortedDict;
                gvWords.DataSource = sortedDict;
                gvWords.DataBind();
                gvWords.Visible = true;
            }
        }


        protected void btnSend_Click(object sender, EventArgs e)
        {            
            var HtmlParsed = "";
            Totals("reset");
            lblErrorMessage.Visible = false;
            var userText = txtInput.Text.Trim();
            if (inputTypes.SelectedValue == "url")
            {
                var response = HtmlParserHelper.GetWebRequestContent(userText);
                //check the status of the web request
                if (response.Error)
                {
                    lblErrorMessage.Visible = true;
                    lblErrorMessage.Text = string.Format("Error in the requested URL: {0} - Message: {1}", response.Status, response.Message);
                    return;
                }
                else
                {
                    userText = response.Content;
                    AnalyzeMetaTags(response.Content);
                    HtmlParsed = HtmlParserHelper.ParseHtml(response.Content);
                }
            }
            else
                gvMetaTags.Visible = false;

            //analyze all links in the text/parse html
            var linksTot = AnalyzeLinks(userText);
            //analyze all words in the text/parse html
            var wordsTot = AnalyzeText(HtmlParsed != "" ? HtmlParsed : userText);

            //Bind totals
            Totals("bind", linksTot, wordsTot);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ResetView();
        }
        protected void ResetView()
        {
            Totals("reset");
            gvMetaTags.Visible = false;
            gvWords.Visible = false;

            txtInput.Text = "";
            txtInput.TextMode = TextBoxMode.MultiLine;
            chkMtKey.Enabled = false;
            chkMtKey.Checked = false;

            chkKeys.Checked = true;
            chkLinks.Checked = true;
            chkStopwords.Checked = true;

            inputTypes.SelectedValue = "text";
            lblErrorMessage.Text = "";
            lblErrorMessage.Visible = false;
        }
    }
}