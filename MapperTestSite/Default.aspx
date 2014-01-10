<%@ Page Trace="False" Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="Mapper" Namespace="Mapper" TagPrefix="asp" %>
<%@ Register TagPrefix="uc" TagName="NumericInput" Src="~/NumericInput.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>
        <fieldset>
            <legend>Test Generation</legend>
            <p>
                <asp:HyperLink ID="hlCanvasImages" runat="server" NavigateUrl="#Details">Generation details</asp:HyperLink>
                <asp:HyperLink ID="hlGraphs" runat="server" NavigateUrl="#Graphs">Graphs with test results</asp:HyperLink>
            </p>

               <p>
                    <asp:Button ID="btnCustom" runat="server" Text="Run custom test as specified below" OnClick="btnCustom_Click" />
               </p>
            </fieldset>

            <fieldset>
                <legend>Test Sets</legend>
                <p>
                    <asp:CheckBox ID="chkFixed" runat="server" Checked="true" Text="Add a fixed set of interesting built in tests" />
                </p>
                <p>
                    <asp:CheckBox ID="chkRandom" runat="server" Checked="false" Text="Add random tests" />
                </p>
                <p>
                <small>
                The fixed set is good for studying a problem in depth. It is created in method AddFixedTests.
                The random tests are good for finding new problems.
                </small>
                </p>
            </fieldset>
            <fieldset>
                <legend>Detailed Output</legend>
                <p>
                    <asp:CheckBox ID="chkShowGenerationDetails" runat="server" Checked="false" Text="Show generation details for successes only" />
                </p>
                <p>
                    <asp:CheckBox ID="chkShowGenerationDetailsFailuresToo" runat="server" Checked="true" Text="Show generation details for successes and failures" />
                </p>
                <p>
                    <asp:CheckBox ID="chkShowCanvasImages" runat="server" Checked="true" Text="Show graphically how images are placed on the canvas" />
                </p>
                <uc:NumericInput id="ImageZoom" runat="server" DefaultInt="2" Label="Image Zoom" />
                <p>
                <small>
                <p>
                If you check the &lt;Show generation details ...&gt; checkboxes, you will see how every single sprite is generated. This takes a lot of room on the page!
                &quot;Successes&quot; are when the program managed to place all images on the canvas. &quot;Failures&quot; are when it failed to do so.
                There are many more failures than successes, so looking at failures makes the page much longer. Look at failures to find ways to
                predict as soon as possible whether it is going to fail to place all images on the canvas.
                </p>
                <p>
                To see the generation details without the images (to shorten the page and to make generation quicker), uncheck 
                &lt;Show graphically how images are placed on the canvas&gt;.                
                </p>
                <p>
                &lt;Image Zoom&gt; lets you zoom in the images that show how each sprite is constructed. If you set this to 4, each image will
                be 4 times bigger than the actual sprites.
                </p>
                </small>
                </p>
            </fieldset>
            <fieldset>
                <legend>Cut offs</legend>

                <uc:NumericInput ID="CutoffEfficiency" runat="server" DefaultFloat="1.0" AllowFloats="true" Label="Cutoff efficiency" />
                <uc:NumericInput id="MaxCandidates" runat="server" DefaultInt="100" Label="Maximum number successful candidates" />
                <p>
                <small>
                Use this section to sacrifice efficiency to gain greater speed. 
                <p />
                The program will stop trying to get a better solution after it has generated &lt;Maximum number successful candidates&gt;
                successful candidates (that is, solutions for packing all rectangles in the enclosing rectangle). Often after 1 or 2
                solutions have been generated, you are unlikely to get a much better one. Set &lt;Maximum number successful candidates&gt;
                to a high number to take away this limit.
                <p />
                The program will also stop trying when it finds a candidate with efficiency greater or equal to 
                &lt;Cutoff efficiency&gt;. Efficiency is &lt;total area all rectangles&gt; / &lt;area enclosing rectangle&gt; - 
                so a perfect solution has efficiency 1.0 and worse solutions have lower efficiencies. You may want to start experimenting with
                cutoff efficiencies between 0.8 and 0.9. To have the program try to achieve the best efficiency without cutoff, set this to 1.0.
                </small>
                </p>
            </fieldset>
            <fieldset>
                <legend>Random Test Specification</legend>

                <uc:NumericInput id="LowestNbrImages" runat="server" DefaultInt="5" Label="Lowest nbr images" />
                <uc:NumericInput ID="HighestNbrImages" runat="server" DefaultInt="10" Label="Highest nbr images" />
                <uc:NumericInput ID="NbrTestsPerNbrImages" runat="server" DefaultInt="1" Label="Nbr tests per nbr images" />
                <p>
                <small>
                This section is only relevant if you checked "Add random tests". This creates a number of random tests. 
                First &lt;Nbr tests per nbr images&gt; sprites will be generated with &lt;Lowest nbr images&gt; images.
                Then the same number of sprites will be generated with one more image. And so on, until 
                &lt;Nbr tests per nbr images&gt; sprites have been generated with &lt;Highest nbr images&gt; images.
                </small>
                </p>
            </fieldset>
            <fieldset>
                <legend>Random Image Specification</legend>

                <uc:NumericInput id="LowestWidth" runat="server" DefaultInt="5" Label="Lowest width" />
                <uc:NumericInput ID="HighestWidth" runat="server" DefaultInt="50" Label="Highest width" />
                <uc:NumericInput ID="AspectRatioDeviation" runat="server" DefaultInt="70" Label="Aspect ratio deviation" MaxLength="2" />
                <p>
                <small>
                This section is only relevant if you checked "Add random tests". Here you specify bounds for the dimensions of the random images.
                All images will be at least &lt;Lowest width&gt; px wide and no more than &lt;Highest width&gt; px wide.
                &lt;Aspect ratio deviation&gt; is a percentage. If it is 70, than the height of each image will be between
                30% and 170% of its width.
                </small>
                </p>
            </fieldset>
        </p>

        <!-- +++++++++++++++++++++++++++++++++ -->

        <asp:PlaceHolder ID="phGenerationDetails" runat="server" EnableViewState="false" Visible="false">
            <a name="Details"></a>
            <h1>Generation Details</h1>
        </asp:PlaceHolder>

        <asp:Repeater id="rptrTests" runat="server" OnItemDataBound="rptrTests_ItemDataBound" EnableViewState="false">
          <ItemTemplate>
              <p>
                  <h2><asp:Literal ID="ltHeading" runat="server"></asp:Literal></h2>
              </p>
              <p><b>Dimensions of images to be placed</b></p>
                <blockquote>
                    <asp:GridView ID="gvRectangleDimensions" AutoGenerateColumns="false" runat="server">
                        <columns>
                            <asp:boundfield datafield="Width" readonly="true" headertext="Width"/>
                            <asp:boundfield datafield="Height" readonly="true" headertext="Height"/>
                        </columns>
                    </asp:GridView>
                </blockquote>
                <p><b>Final sprite</b></p>
                <blockquote>
                    <p>
                        Dimensions: 
                        <asp:Literal runat="server" ID="ltFinalWidth"></asp:Literal> x <asp:Literal runat="server" ID="ltFinalHeight"></asp:Literal>,
                        area: <asp:Literal runat="server" ID="ltFinalArea"></asp:Literal>,
                        efficiency: <asp:Literal runat="server" ID="ltFinalEfficiency"></asp:Literal>
                    </p>

                    <div>
                        <div style="float: left;">
                            <asp:GridView ID="gvSpriteImageOffsets" AutoGenerateColumns="false" runat="server">
                                <columns>
                                    <asp:boundfield datafield="X" readonly="true" headertext="X"/>
                                    <asp:boundfield datafield="Y" readonly="true" headertext="Y"/>
                                    <asp:templatefield headertext="Width">
                                        <ItemTemplate>
                                            <%# ((IMappedImageInfo)Container.DataItem).ImageInfo.Width %>
                                        </ItemTemplate>
                                    </asp:templatefield>
                                    <asp:templatefield headertext="Height">
                                        <ItemTemplate>
                                            <%# ((IMappedImageInfo)Container.DataItem).ImageInfo.Height%>
                                        </ItemTemplate>
                                    </asp:templatefield>
                                </columns>
                            </asp:GridView>
                        </div>
                        <div style="float: left; margin-left: 10px;">
                            <asp:Image ID="imgSprite" runat="server" />
                            <p>
                            <small>Image zoomed in <%# _imageZoom %> times</small>
                            </p>
                        </div>
                        <div style="clear:both;"></div>
                    </div>

                </blockquote>

                <p><b>Steps to arrive at final sprite:</b></p>
                <ol>
                <asp:Repeater id="rptrIterations" runat="server">
                    <ItemTemplate>
                        <li style="margin-bottom: 10px;">
                            Canvas - 
                            <asp:Literal runat="server" Text='<%# Eval("MaxCanvasWidth") %>'></asp:Literal> x 
                            <asp:Literal runat="server" Text='<%# Eval("MaxCanvasHeight") %>' ></asp:Literal>,
                            area: <asp:Literal runat="server" Text='<%# Eval("MaxCanvasArea") %>' ></asp:Literal>
                            |
                            <asp:PlaceHolder runat="server" Visible='<%# (IterationResult)Eval("Result") == IterationResult.BiggerThanBestSprite %>'>
                                Bigger than best sprite so far
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" Visible='<%# (IterationResult)Eval("Result") == IterationResult.SmallerThanCombinedImages %>'>
                                Smaller than combined size of all images
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" Visible='<%# (IterationResult)Eval("Result") == IterationResult.Failure %>'>
                                Failed to place all images on the canvas
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" Visible='<%# (IterationResult)Eval("Result") == IterationResult.Success %>'>
                                    Resulting sprite - 
                                    <asp:Literal runat="server" Text='<%# Eval("IntermediateSpriteWidth") %>'></asp:Literal> x 
                                    <asp:Literal runat="server" Text='<%# Eval("IntermediateSpriteHeight") %>'></asp:Literal>,
                                    area: <asp:Literal runat="server" Text='<%# Eval("IntermediateSpriteArea", "{0:N0}") %>'></asp:Literal>,
                                    efficiency: <asp:Literal runat="server" Text='<%# Eval("IntermediateSpriteEfficiency", "{0:p}") %>'></asp:Literal>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder runat="server" 
                                Visible='<%# ((IterationResult)Eval("Result") == IterationResult.Success) || (_showGenerationDetailsOfFailures && ((IterationResult)Eval("Result") == IterationResult.Failure)) %>'>
                                <ol style="list-style-type:lower-alpha">
                                <asp:Repeater id="rptrCanvasImages" runat="server" DataSource='<%# Eval("ImageDetails") %>'>
                                    <ItemTemplate>
                                        <li>
                                            <asp:PlaceHolder runat="server" Visible='<%# (int)Eval("XOffset") > -1 %>'>
                                            <p>
                                            <b>Placed image <%# Eval("Width") %> x <%# Eval("Height") %>, at position <%# Eval("XOffset") %> x <%# Eval("YOffset")%></b>
                                            </p>
                                            <p>
                                            <img src="<%# Eval("ImageUrl") %>" alt="" style="margin-bottom: -5px;" />
                                            </p>
                                            </asp:PlaceHolder>

                                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%# (int)Eval("XOffset") == -1 %>'>
                                            <p>
                                            <b>Failed to place image <%# Eval("Width") %> x <%# Eval("Height") %></b>
                                            </p>
                                            <p>
                                            <img src="<%# Eval("ImageUrl") %>" alt="" style="margin-bottom: -5px;" />
                                            </p>
                                            </asp:PlaceHolder>

                                            <p>
                                            <small>Image zoomed in <%# _imageZoom %> times</small>
                                            </p>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                </ol>
                            </asp:PlaceHolder>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
               </ol>
          </ItemTemplate>
       </asp:Repeater>

        <!-- +++++++++++++++++++++++++++++++++++++ -->

       <a name="Graphs"></a>
       <h1 runat="server" id="h1TestResults">Test Results</h1>
        <p>
            The results shown here compare the speed and efficiency
            of the IMapper implementations listed in the <i>mappers</i> array declared in the <i>Generate</i> method in Default.aspx.cs.
            Currently these are <i>MapperHorizontalOnly</i> (which simply strings images into a sprite horizontally), 
            <i>MapperVerticalOnly</i> (which strings them vertically), and
            <i>MapperOptimalEfficiency</i> (which trades off CPU ticks against higher sprite efficiency).
            If you've done your own implementation of IMapper, you could add that to the <i>mappers</i> array
            to compare its performance.
        </p>
        <p>
            In this context, speed means how long it takes on average for an algorithm to produce a sprite, in elapsed ticks.
            Efficiency means &lt;total sprite area&gt; / &lt;total area of the images contained in the sprite&gt;. Best efficiency would be 100%,
            in which case the contained images take up all the space in the sprite and there is no wasted space.
        </p>
       <p>
           <asp:Literal ID="ltPathToCSV" runat="server"></asp:Literal>
       </p>
       <p>
            <asp:GridView ID="gvOverallResults" AutoGenerateColumns="true" runat="server">
            </asp:GridView>
       </p>

        <asp:PlaceHolder ID="phCharts" runat="server" EnableViewState="false"></asp:PlaceHolder>

    </div>
    </form>
</body>
</html>
