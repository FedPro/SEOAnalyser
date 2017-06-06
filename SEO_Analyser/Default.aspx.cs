using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace SEO_Analyser
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string Sortkey = "SortExpr_";
                ViewState[Sortkey + "gvWords"] = "SortbyWord ASC";
                ViewState[Sortkey + "gvMetaTags"] = "SortbyWord ASC";
            }
        }

        protected int AnalyzeLinks(string pText)
        {
            ///reg to extract links
            var regLinks = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.IgnoreCase);
            return regLinks.Matches(pText).Count;
        }

        protected Dictionary<string, int> Analyzer(string InputTxt, bool pCountOccurrences)
        {
            // initialization          
            var stopW = new string[] { }; ;
            if (chkStopwords.Checked) // Load the stop words only in case we need filter out the text
                stopW = AnalyzerUtilities.GetStopWordsList();
            // dictionary to store and count the words found
            Dictionary<string, int> wordsCounter = new Dictionary<string, int>();
            ////////////////////////////////////////////////////////////////////////////////////
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
                wordsCounter = AnalyzerUtilities.AddWordToList(w, wordsCounter, pCountOccurrences);
            }
            ////////////////////////////////////////////////////////////////////////////////////
            return wordsCounter;
        }


        protected void BindData(Dictionary<string, int> pData, string pGridViewID, string pDicKey, string pSortExprKey)
        {
            //Binding data
            ViewState[pDicKey] = pData;
            BindOrderedData(ViewState[pSortExprKey].ToString(), pDicKey, pGridViewID);
            //return wordsCounter.Count();
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
            //get the Client Id of the grid View
            string gvID = ((System.Web.UI.Control)sender).ClientID;
            //set the key for the View state to get the Sort expression
            var SortKey = "SortExpr_" + gvID;

            //get the current state of the sort expression
            string[] State_SortExpressions = ViewState[SortKey].ToString().Split(' ');

            // check about which column
            if (State_SortExpressions[0] == e.SortExpression)
            {
                ViewState[SortKey] = State_SortExpressions[1] == "ASC" ? e.SortExpression + " " + "DESC" : e.SortExpression + " " + "ASC";

            }
            // in case of another column - sort by ASC
            else
            {
                ViewState[SortKey] = e.SortExpression + " " + "ASC";
            }            
            BindOrderedData(ViewState[SortKey].ToString(), "dic_"+gvID, gvID);
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

        protected void BindOrderedData(string expr, string dicKey, string GridViewID)
        {
            if (ViewState[dicKey] != null)
            {
                // Get the data from ViewState
                Dictionary<string, int> wordsCounter = (Dictionary<string, int>)ViewState[dicKey];
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
                ViewState[dicKey] = sortedDict;

                // Find control on page
                GridView gvControl = (GridView)FindControl(GridViewID);
                if (gvControl != null)
                {
                    gvControl.DataSource = sortedDict;
                    gvControl.DataBind();
                    gvControl.Visible = true;
                }
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
                    ///pass the meta tags content row to the Analyzer                                                  
                    BindData(Analyzer(HtmlParserHelper.GetMetaTagsContent(response.Content), chkMtKey.Checked), "gvMetaTags", "dic_gvMetaTags", "SortExpr_gvMetaTags");
                    HtmlParsed = HtmlParserHelper.ParseHtml(response.Content);
                }
            }
            else
                gvMetaTags.Visible = false;

            //analyze all links in the text/parse html
            var linksTot = 0;
            if (chkLinks.Checked)
                linksTot = AnalyzeLinks(userText);

            //analyze all words in the text/parse html
            var wordsTot = 0;
            var list = Analyzer(HtmlParsed != "" ? HtmlParsed : userText, chkKeys.Checked);
            wordsTot = list.Count();
            BindData(list, "gvWords", "dic_gvWords", "SortExpr_gvWords");

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