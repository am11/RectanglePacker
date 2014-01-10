using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class NumericInput : System.Web.UI.UserControl
{
    public bool AllowFloats { get; set; }
    
    public int DefaultInt { get; set; }
    public float DefaultFloat { get; set; }

    public string Label { set { lbLabel.Text = value; } }
    
    public int IntValue { 
        get {
            if (AllowFloats) { throw new Exception("Cannot use InputValue get if floats allowed"); }
            return Convert.ToInt32(txtInput.Text); 
        }
        set { txtInput.Text = value.ToString(); } 
    }

    public float FloatValue
    {
        get { return float.Parse(txtInput.Text); }
        set { txtInput.Text = value.ToString(); }
    }

    private int _maxLength = 4;
    public int MaxLength
    {
        get { return _maxLength; }
        set { _maxLength = value; }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            txtInput.Text = AllowFloats ? DefaultFloat.ToString() : DefaultInt.ToString();
            txtInput.MaxLength = _maxLength;

            revInteger.Enabled = !AllowFloats;
            revFloat.Enabled = AllowFloats;
        }
    }
}

