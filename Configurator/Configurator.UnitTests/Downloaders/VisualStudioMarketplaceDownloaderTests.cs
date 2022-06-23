using System.Threading.Tasks;
using Configurator.Configuration;
using Configurator.Downloaders;
using Configurator.Utilities;
using Emmersion.Http;
using Moq;
using Shouldly;
using Xunit;

namespace Configurator.UnitTests.Downloaders
{
    public class VisualStudioMarketplaceDownloaderTests : UnitTestBase<VisualStudioMarketplaceDownloader>
    {
        [Fact]
        public async Task WhenStartingANewTest()
        {
            string argsJson = RandomString();
            var args = new VisualStudioMarketplaceDownloaderArgs
            {
                Publisher = RandomString(),
                ExtensionName = RandomString()
            };

            string fileUrl =
                $"https://marketplace.visualstudio.com/_apis/public/gallery/publishers/{args.Publisher}/vsextensions/{args.ExtensionName}/1.4.1/vspackage";
            string filePath = $"{args.Publisher}.{args.ExtensionName}.vsix";

            var httpResponse = new HttpResponse(200, new HttpHeaders(), BuildSampleHtml(args));

            GetMock<IJsonSerializer>().Setup(x => x.Deserialize<VisualStudioMarketplaceDownloaderArgs>(argsJson))
                .Returns(args);

            IHttpRequest? capturedHttpRequest = null;
            GetMock<IHttpClient>().Setup(x => x.ExecuteAsync(IsAny<HttpRequest>()))
                .Callback<IHttpRequest>(httpRequest => capturedHttpRequest = httpRequest)
                .ReturnsAsync(httpResponse);

            GetMock<IResourceDownloader>().Setup(x => x.DownloadAsync(fileUrl, filePath))
                .ReturnsAsync(filePath);

            var downloadedFilePath = await BecauseAsync(() => ClassUnderTest.DownloadAsync(argsJson));

            It("calls the Visual Studio Marketplace", () =>
            {
                capturedHttpRequest.ShouldNotBeNull().ShouldSatisfyAllConditions(x =>
                {
                    x.Url.ShouldBe($"https://marketplace.visualstudio.com/items?itemName={args.Publisher}.{args.ExtensionName}");
                    x.Method.ShouldBe(HttpMethod.GET);
                });
            });

            It("returns the downloaded file's path", () => { downloadedFilePath.ShouldBe(filePath); });
        }

        private string BuildSampleHtml(VisualStudioMarketplaceDownloaderArgs args)
        {
            return SampleMarketplaceExtensionHtmlExtracted_2022_06_21
                .Replace("Shanewho", args.Publisher)
                .Replace("IHateRegions", args.ExtensionName);
        }
        
        private string SampleMarketplaceExtensionHtmlExtracted_2022_06_21
        {
            get =>
                @"
<html lang=""en-us"">

<head>
	<meta name=""description""
		content=""Extension&#32;for&#32;Visual&#32;Studio&#32;-&#32;Regions&#32;Suck.&#32;You&#32;need&#32;this."" />
	<meta name=""keywords"" content=""regions&#32;#regions&#32;#region&#32;region&#32;c#&#32;vb&#32;vb.net"" />
	<meta property=""og:title"" content=""I&#32;Hate&#32;#Regions&#32;-&#32;Visual&#32;Studio&#32;Marketplace"" />
	<meta property=""og:type"" content=""website"" />
	<meta property=""og:url"" content=""https://marketplace.visualstudio.com/items?itemName=Shanewho.IHateRegions"" />
	<meta property=""og:image""
		content=""https://shanewho.gallerycdn.vsassets.io/extensions/shanewho/ihateregions/1.4.1/1523844662971/Microsoft.VisualStudio.Services.Icons.Default"" />
	<meta property=""og:description""
		content=""Extension&#32;for&#32;Visual&#32;Studio&#32;-&#32;Regions&#32;Suck.&#32;You&#32;need&#32;this."" />
	<meta property=""twitter:card"" content=""summary"" />
	<meta property=""twitter:site"" content=""@VisualStudio"" />
	<link rel=""canonical"" href=""https://marketplace.visualstudio.com/items?itemName=Shanewho.IHateRegions"" />
	<style>
		html {
			overflow-y: scroll;
		}

		body {
			font-family: wf_segoe-ui, Helvetica Neue, Helvetica, Arial, Verdana;
			color: rgba(0, 0, 0, 1);
			font-size: 12px;
			background-color: rgba(255, 255, 255, 1);
			margin: 0;
			padding: 0;
		}

		/* L0 header styles start*/
		.uxservices-header {
			height: 34px;
			background-color: black;
		}

		a.skip-main {
			left: -999px;
			position: absolute;
			top: auto;
			width: 1px;
			height: 1px;
			overflow: hidden;
			z-index: -999;
		}

		a.skip-main:focus,
		a.skip-main:active {
			color: #0078D4;
			background-color: #FFFFFF;
			position: absolute;
			left: auto;
			top: auto;
			width: 122px;
			height: 33px;
			overflow: auto;
			padding-left: 12px;
			padding-right: 12px;
			padding-top: 7px;
			text-align: center;
			font-size: 14px;
			font-weight: 500;
			font-family: 'Segoe UI';
			line-height: 20px;
			z-index: 999;
		}

		a.skip-main:hover {
			color: #005BA1;
			background-color: #F2F2F2;
		}

		.uxservices-header div.upperBandContent {
			background-color: black;
			color: white;
			line-height: 17px;
			width: 100%;
			margin: 0 auto;
			box-sizing: border-box;
			max-width: 1160px;
			padding: 0px;
		}

		.uxservices-header div.upperBandContent .left {
			padding: 5px 0 0 0;
		}

		.uxservices-header div.upperBandContent .left .vs-logo-header {
			height: 31px;
			padding-top: 5px;
		}

		.uxservices-header div.upperBandContent .left .marketplacetext-header {
			vertical-align: 9px;
			padding-top: 5px;
		}

		.uxservices-header div.upperBandContent .right {
			display: inline-block;
			position: relative;
			background-color: black;
			font-size: 11px;
			padding-top: 10px;
		}

		.uxservices-header div.upperBandContent .right .signIn {
			font-weight: 600;
			float: left;
		}

		.uxservices-header .marketPlaceLogoLink {
			white-space: pre;
			font-size: 16px;
			line-height: 16px;
			vertical-align: -4px;
			display: inline-block;
			color: white;
		}

		.uxservices-header .marketPlaceLogoLink.vs-brand-icon {
			width: 30px;
			height: 30px;
			background: url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Header/ImageSprite.png) no-repeat -250px 0 !important;
		}

		.uxservices-header .right a {
			font-family: wf_segoe-ui, -apple-system, "".SFNSText-Regular"", ""San Francisco"", ""Roboto"", ""Helvetica Neue"", ""Lucida Grande"", sans-serif;
			font-size: inherit;
		}

		.uxservices-header div.upperBandContent .scarabLink {
			margin-right: 0.23em;
		}

		#Fragment_SearchBox {
			display: inline-block;
			margin-left: 25px;
			height: 13px;
		}

		#Fragment_SearchBox .header-search-button {
			height: 20px;
			width: 20px;
			background: url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Header/ImageSprite.png) no-repeat -195px 0 !important;
			cursor: pointer;
			border: none;
			margin-left: 3px;
			padding: 0px;
		}

		#Fragment_SearchBox .header-search-button:focus {
			outline-color: rgb(77, 144, 254);
			outline-style: auto;
			outline-width: 5px;
		}

		#Fragment_SearchBox .header-search-textbox {
			vertical-align: top;
			background-color: #6a6a6a;
			color: #ffffff;
			padding-left: 2px;
		}


		/* l0 header ends*/

		.tab {
			overflow: hidden;
			border-bottom: 1px solid #ccc;
			background-color: transparent;
			margin-bottom: 25px;
		}

		/* Style the buttons that are used to open the tab content */
		.tab button {
			background-color: transparent;
			float: left;
			border: none;
			outline: none;
			cursor: pointer;
			padding: 0px 16px;
			font-size: 16px;
			height: 40px;
			border-width: 1px;
			border-style: solid;
			border-color: transparent
		}

		/* Change background color of buttons on hover */
		.tab button:hover {
			color: #106ebe;
		}

		.tab button.selected {
			color: #106ebe;
			border-bottom: 2px solid #106ebe;
		}

		.tab button:focus,
		.tab button.selected:focus {
			border-color: #000000;
		}

		/* Style the tab content */
		.tabcontent {
			padding: 6px 12px;
		}

		.bowtie-icon,
		i.bowtie-icon {
			font-family: ""Bowtie"";
			font-size: 14px;
			speak: none;
			display: inline-block;
			font-style: normal;
			font-weight: normal;
			font-variant: normal;
			text-transform: none;
			text-align: center;
			text-decoration: none;
			line-height: 16px;
			-webkit-font-smoothing: antialiased;
			-moz-osx-font-smoothing: grayscale;
		}

		.bowtie-icon:disabled,
		i.bowtie-icon:disabled {
			opacity: 0.5;
		}

		.bowtie-brand-visualstudio::before {
			content: ""\E91D"";
		}

		.bowtie-install::before {
			content: ""\E92C"";
		}

		.bowtie-search::before {
			content: ""\E986"";
		}

		.bowtie-navigate-external::before {
			content: ""\E9D0"";
		}

		.bowtie-status-info::before {
			content: ""\EA08"";
		}

		.bowtie-status-info {
			color: rgba(0, 120, 212, 1);
			color: var(--communication-background, rgba(0, 120, 212, 1));
		}

		* {
			box-sizing: border-box;
			-webkit-box-sizing: border-box;
			-moz-box-sizing: border-box;
		}

		a:not(.ms-Button) {
			color: rgba(0, 120, 212, 1);
			color: var(--communication-foreground, rgba(0, 120, 212, 1));
			text-decoration: none;
			cursor: pointer;
		}

		a:not(.ms-Button):hover {
			color: rgba(0, 120, 212, 1);
			color: var(--communication-foreground, rgba(0, 120, 212, 1));
			text-decoration: underline;
		}

		a:not(.ms-Button):visited {
			color: rgba(0, 120, 212, 1);
			color: var(--communication-foreground, rgba(0, 120, 212, 1));
		}

		a:not(.ms-Button):active {
			color: rgba(0, 120, 212, 1);
			color: var(--communication-foreground, rgba(0, 120, 212, 1));
		}

		button {
			font-family: ""Segoe UI VSS (Regular)"", ""Segoe UI"", ""-apple-system"", BlinkMacSystemFont, Roboto, ""Helvetica Neue"", Helvetica, Ubuntu, Arial, sans-serif, ""Apple Color Emoji"", ""Segoe UI Emoji"", ""Segoe UI Symbol"";
			height: 30px;
			border: 1px solid;
			border-color: rgba(200, 200, 200, 1);
			border-color: rgba(var(--palette-neutral-20, 200, 200, 200), 1);
			background-color: rgba(248, 248, 248, 1);
			background-color: rgba(var(--palette-neutral-2, 248, 248, 248), 1);
			color: rgba(51, 51, 51, 1);
			color: rgba(var(--palette-neutral-80, 51, 51, 51), 1);
			cursor: pointer;
			outline: 0;
			padding: 2px 12px 2px 12px;
		}

		button:hover:not(.ms-Button):not(.bolt-button):not(.link-as-button),
		button:focus:not(.ms-Button):not(.bolt-button):not(.link-as-button) {
			border-color: rgba(166, 166, 166, 1);
			border-color: rgba(var(--palette-neutral-30, 166, 166, 166), 1);
			border-style: dotted;
		}

		.pricingTab-loader-container,
		.qnaTab-loader-container,
		.rnrTab-loader-container,
		.versionHistoryTab-loader-container {
			text-align: center;
		}

		.pricingTab-loader-container .loader,
		.qnaTab-loader-container .loader,
		.rnrTab-loader-container .loader,
		.versionHistoryTab-loader-container .loader {
			display: inline-block;
		}

		/* Styles for bread crumb */
		.bread-crumb-container {
			font-size: 13px;
			color: #FFFFFF;
			height: 45px;
			line-height: 45px;
		}

		.bread-crumb-container .member {
			text-decoration: none;
		}

		.bread-crumb-container .separator {
			padding: 0 5px;
		}

		.bread-crumb-container a:visited {
			color: #FFFFFF;
		}

		.bread-crumb-container a {
			color: #FFFFFF;
		}

		.bread-crumb-container a:hover {
			color: #FFFFFF;
		}

		.item-details-control-root .breadcrumb {
			background-color: #232323;
			color: #FFFFFF;
			height: 45px;
			line-height: 45px;
			font-size: 13px;
			min-width: 1250px;
		}

		.item-details-control-root .breadcrumb .vsCodeDownloadLinkContainer {
			float: right;
			color: inherit;
		}

		.item-details-control-root .breadcrumb .vsCodeDownloadLink {
			color: #0090ff;
		}

		/* end of bread crumb styles */

		/* Styles for item banner */
		.ux-section-banner {
			padding: 16px 0px;
			background-color: #eff1f3;
			min-width: 1250px;
		}

		.item-details-control-root .ux-section-banner {
			padding: 32px 0px;
			background-color: #eff1f3;
		}

		.gallery-centered-content {
			width: 1160px;
			margin-left: auto;
			margin-right: auto;
			padding-left: 10px;
			padding-right: 10px;
		}

		table {
			border-collapse: collapse;
			border-spacing: 0;
			margin: 0;
			padding: 0;
			border: 0;
		}

		.item-details-control-root .item-img {
			text-align: center;
			vertical-align: top;
			width: 132px;
		}

		.item-details-control-root .item-img img {
			max-width: 128px;
			position: relative;
			visibility: hidden;
		}

		.item-details-control-root .item-header .item-header-content {
			font-size: 14px;
			margin-left: 32px;
		}

		.item-details-control-root .ux-item-name {
			font-size: 26px;
			font-weight: 600;
			display: inline-block;
			padding-right: 8px;
		}

		.item-details-control-root .ux-item-second-row-wrapper {
			margin-top: 4px;
		}

		.item-details-control-root .ux-item-second-row-wrapper>div {
			display: inline-block;
		}

		.item-details-control-root .ux-item-second-row-wrapper>div:nth-child(1) {
			margin-right: 10px;
		}

		.item-details-control-root .ux-item-second-row-wrapper>.ux-item-rating {
			margin-left: 10px;
			margin-right: 10px;
		}

		.item-details-control-root .ux-item-second-row-wrapper>.item-price-category {
			margin-left: 10px;
			margin-right: 10px;
		}

		.item-details-control-root .item-header h1,
		.item-details-control-root .item-header h2 {
			margin: 0px;
			padding: 0px;
			display: inline-block;
			font-size: 0em;
			font-weight: normal;
		}

		.item-details-control-root .item-header .dark {
			color: #FFFFFF;
		}

		.item-details-control-root .ux-item-rating {
			font-size: 14px;
		}

		.item-details-control-root .ux-item-publisher {
			font-size: 18px;
		}

		.item-details-control-root .ux-item-publisher-link {
			font-size: 18px;
			font-weight: 600;
		}

		.item-details-control-root .ux-item-shortdesc {
			margin: 20px 0;
			width: 608px;
			line-height: 1.5;
			overflow: hidden;
		}

		.item-details-control-root .dark .item-banner-focussable-child-item:focus {
			outline: 1px dotted white;
		}

		.item-details-control-root .item-header .item-header-content.light .installHelpInfo a {
			color: #000000;
			text-decoration: underline;
			border-color: #000000;
		}

		.item-details-control-root .item-header .item-header-content.dark .installHelpInfo a {
			color: #FFFFFF;
			text-decoration: underline;
		}

		.ux-section-details-tabs .version-history-top-container {
			width: 100%
		}

		/* not unpublished */

		.item-details-control-root .ux-button.install {
			position: relative;
			font-family: ""Segoe UI"", ""Segoe UI Web (West European)"", ""Segoe UI"", -apple-system, BlinkMacSystemFont, Roboto, ""Helvetica Neue"", sans-serif;
			-webkit-font-smoothing: antialiased;
			font-size: 14px;
			font-weight: 400;
			box-sizing: border-box;
			display: inline-block;
			text-align: center;
			cursor: pointer;
			vertical-align: top;
			padding-top: 0px;
			padding-right: 16px;
			padding-bottom: 0px;
			padding-left: 16px;
			min-width: 80px;
			height: 32px;
			background-color: rgb(244, 244, 244);
			color: rgb(51, 51, 51);
			user-select: none;
			outline: transparent;
			border-width: 1px;
			border-style: solid;
			border-color: transparent;
			border-image: initial;
			text-decoration: none;
			border-radius: 0px;
		}

		.item-details-control-root .item-header-content .install-button-container .ux-button.install.buttonDisabled {
			background-color: #AAAAAA;
			color: #EEEEEE;
			opacity: 1;
			border: 1px solid #E6E6E6;
		}

		.item-details-control-root .item-header-content .install-button-container .ux-button.install.buttonDisabled:hover {
			cursor: default;
			opacity: 1;
		}

		.item-details-control-root .dark .ux-button.install {
			background-color: #107c10;
			color: #FFFFFF;
			min-width: 120px;
			border: 1px solid #FFFFFF;
		}

		.item-details-control-root .dark .ux-button.install:hover {
			background-color: #AAAAAA;
			opacity: 0.6;
		}

		.item-details-control-root .light .ux-button.install {
			background-color: #107c10;
			color: #FFFFFF;
			min-width: 120px;
			border: 1px solid #666666;
		}

		.item-details-control-root .light .ux-button.install:hover,
		.item-details-control-root .light .ux-button.install:focus {
			background-color: #074507;
		}

		.item-details-control-root .dark .ux-button.install:focus,
		.item-details-control-root .light .ux-button.install:focus {
			border: 1px dotted;
		}


		.vscode-install-title {
			font-size: 16px;
			font-weight: bold;
			padding: 5px 0px;
		}

		.vscode-install-info-container {
			font-size: 13px;
			padding-top: 4px;
		}

		.vscode-command-container {
			height: 35px;
			padding: 1px 0px;
			float: left;
		}

		.vscode-command-input {
			padding: 2px;
			height: 100%;
			padding-left: 9px;
			padding-right: 9px;
			font-family: monospace;
			border: 1px solid #a9a9a9;
			float: left;
			text-overflow: ellipsis;
		}

		.copy-to-clipboard-button {
			height: 100%;
			padding: 2px 14px 4px 14px;
			margin-left: -1px;
			border: 1px solid #a9a9a9;
			border-radius: 0px;
			float: left;
			cursor: pointer;
		}

		.light .copy-to-clipboard-button:focus {
			outline: 1px dotted #666;
		}

		.dark .copy-to-clipboard-button:focus {
			outline: 1px dotted white;
		}

		.copied-display-div {
			font-size: 11px;
			background-color: #000000;
			padding: 2px 4px;
			color: #ffffff;
			text-align: center;
			border-radius: 5px;
			border: 1px solid #ffffff;
			width: 120px;
			display: none;
			position: absolute;
			top: 35px;
			right: -50px;
		}

		.copied-display-container {
			float: left;
			position: relative;
		}

		.vscode-moreinformation {
			border-left: 1px solid;
			margin-left: 15px;
			padding-left: 10px;
			float: left;
			margin-top: 9px;
		}

		.vscode-moreinformation.dark {
			color: inherit;
			text-decoration: underline;
		}

		.vscode-moreinformation.light {
			color: #007acc;
			border-color: #000000;
		}

		/* End of item banner styles. */

		/* Styles for item details overview */
		.item-details-control-root .ux-section-details {
			margin: 24px 0 34px 0px;
			font-size: 13px;
		}

		.item-details-control-root .ux-section-details .itemdetails-section-header {
			font-size: 16px;
			font-weight: bold;
			color: #555;
			padding-bottom: 16px;
		}

		.item-details-control-root .ux-section-details .ux-section-details-table {
			width: 100%;
			table-layout: fixed;
		}

		.item-details-control-root .ux-itemdetails-left {
			vertical-align: top;
			font-size: 14px;
			padding-right: 20px;
		}

		.item-details-control-root .ux-itemdetails-right {
			width: 34.45%;
			vertical-align: top;
			padding: 0 0 0 20px;
		}

		.item-details-control-root .itemDetails-right {
			display: none;
		}

		.item-details-control-root .markdown {
			color: #333;
			line-height: 1.6;
			position: relative;
		}

		.item-details-control-root .markdown>*:first-child {
			margin-top: 0 !important;
		}

		.item-details-control-root .markdown .link-header:target:before {
			content: """";
			display: block;
			height: 250px;
			margin: -250px 0 0;
		}

		.link-as-button {
			height: initial;
		}

		.link-as-button:hover {
			text-decoration: underline;
		}

		/* End of item details overview styles. */

		/* Styles for footer */
		#ux-footer {
			line-height: 1.2em;
		}

		#ux-footer:after {
			content: """";
			display: table;
			clear: both;
		}

		#ux-footer a {
			color: #595958;
		}

		#ux-footer a:hover {
			color: #3399ff;
		}

		#ux-footer ul.links {
			padding: 0;
			line-height: 1.2em;
		}

		#ux-footer .linkList>ul>li {
			list-style-type: none;
			background: none;
			padding: 4px 0;
			margin: 0;
		}

		#ux-footer #baseFooter {
			width: 1160px;
			padding: 15px 10px;
			margin: 0 auto;
		}

		#ux-footer #baseFooter:after {
			content: """";
			display: table;
			clear: both;
		}

		#ux-footer #baseFooter li {
			display: inline-block;
			list-style-type: none;
			margin: 0 15px 0 0;
		}

		#ux-footer #baseFooter #Fragment_BaseFooterLinks {
			text-align: right;
		}

		#ux-footer #baseFooter #rightBaseFooter {
			float: right;
			width: 18%;
			font-family: wf_segoe-ui, Tahoma, Helvetica, Sans-Serif;
			color: #595958;
			font-weight: bold;
		}

		#ux-footer span.microsoftLogo {
			display: inline-block;
			margin-left: 16px;
		}

		.microsoftLogo {
			background: url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Header/ImageSprite.png) no-repeat -93px 0 !important;
		}

		.microsoftLogo {
			width: 70px;
			height: 14px;
			overflow: hidden;
		}

		/* End of footer styles. */

		/* Styles for markdown */
		.markdown img {
			max-width: 100%;
		}

		.markdown h1,
		.markdown h2 {
			line-height: 1.4;
			margin-top: 1em;
			margin-bottom: 16px;
		}

		.markdown h1 {
			font-size: 1.5em;
			line-height: 1.2em;
			border-bottom: 1px solid #eee;
			margin-top: 2em;
			padding-bottom: 10px;
		}

		.markdown h2 {
			font-size: 1.375em;
			line-height: 1.2em;
			border-bottom: 1px solid #eee;
			margin-top: 2em;
			padding-bottom: 10px;
		}

		.markdown h3 {
			font-size: 1.25em;
		}

		.markdown h4 {
			font-size: 1.125em;
		}

		.markdown h5 {
			font-size: 1.0em;
		}

		.markdown blockquote {
			padding: 0 15px;
			color: #777;
			border-left: 4px solid #ddd;
			margin: 0;
		}

		.markdown pre {
			padding: 16px;
			overflow: auto;
			font-size: 85%;
			line-height: 1.45;
			background-color: #f7f7f7;
			border-radius: 3px;
			word-wrap: normal;
			font-family: Consolas, ""Liberation Mono"", Menlo, Courier, monospace;
		}

		.markdown code,
		.markdown tt {
			padding: 0;
			background-color: rgba(0, 0, 0, 0.04);
			margin: 0;
			padding-top: 0.2em;
			padding-bottom: 0.2em;
			border-radius: 3px;
			font-family: Consolas, ""Liberation Mono"", Menlo, Courier, monospace;
		}

		.markdown pre>code {
			background-color: inherit;
		}

		.markdown table {
			display: block;
			width: 100%;
			overflow: auto;
			word-break: normal;
			word-break: keep-all;
		}

		.markdown table th {
			font-weight: bold;
		}

		.markdown table th,
		.markdown table td {
			padding: 6px 13px;
			border: 1px solid #ddd;
		}

		.markdown table tr {
			background-color: #fff;
			border-top: 1px solid #ccc;
		}

		.markdown table tr:nth-child(2n) {
			background-color: #f8f8f8;
		}

		/* End of markdown styles. */

		.main-content.item-details-main-content {
			min-height: 950px;
		}

		.item-details-main-content {
			background-color: #FFFFFF;
		}

		.ms-Fabric {
			-moz-osx-font-smoothing: grayscale;
			-webkit-font-smoothing: antialiased;
			color: #333333;
			font-family: ""Segoe UI Web (West European)"", ""Segoe UI"", -apple-system, BlinkMacSystemFont, ""Roboto"", ""Helvetica Neue"", sans-serif;
			font-size: 14px;
		}

		.ms-Fabric button {
			font-family: inherit;
		}

		.ux-section-details-tabs {
			margin-top: -20px;
		}

		@font-face {
			font-family: wf_segoe-ui;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.eot"");
			src: local(""Segoe UI Light""), local(""Segoe Light""), local(""Segoe WP Light""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.ttf"") format(""truetype"");
			font-weight: 200;
			font-style: normal;
			-webkit-font-smoothing: antialiased;
		}

		#survey-container {
			position: relative;
			background-color: #f5ebc5;
			justify-content: space-between;
			text-align: center;
		}

		#survey-content-container {
			margin: 0;
			padding-left: 5%;
			padding-top: 8px;
			padding-bottom: 8px;
			padding-left: 30%;
			padding-right: 15%;
		}

		#survey-content-text {
			color: #000000 !important;
			display: table-cell;
			vertical-align: middle;
			padding: 0;
			font-family: Segoe UI, SegoeUI, Arial, sans-serif;
			font-style: normal;
			font-weight: normal;
			font-size: 13px;
			line-height: 16px;
		}

		#survey-info-icon {
			display: table-cell;
			padding: 4px;
			width: 24px;
			height: 24px;
			line-height: 0;
			text-align: left;
		}

		@font-face {
			font-family: wf_segoe-ui;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/semilight/latest.eot"");
			src: local(""Segoe UI Semilight""), local(""Segoe Semilight""), local(""Segoe WP Semilight""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/semilight/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/semilight/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/semilight/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/semilight/latest.ttf"") format(""truetype"");
			font-weight: 300;
			font-style: normal;
		}

		@font-face {
			font-family: wf_segoe-ui;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/normal/latest.eot"");
			src: local(""Segoe UI""), local(""Segoe""), local(""Segoe WP""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/normal/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/normal/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/normal/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/normal/latest.ttf"") format(""truetype"");
			font-weight: normal;
			font-style: normal;
		}

		@font-face {
			font-family: wf_segoe-ui;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.eot"");
			src: local(""Segoe UI Semibold""), local(""Segoe Semibold""), local(""Segoe WP Semibold""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.ttf"") format(""truetype"");
			font-weight: 500;
			font-style: normal;
		}

		@font-face {
			font-family: wf_segoe-ui;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.eot"");
			src: local(""Segoe UI Semibold""), local(""Segoe Semibold""), local(""Segoe WP Semibold""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/Semibold/latest.ttf"") format(""truetype"");
			font-weight: 600;
			font-style: normal;
		}

		@font-face {
			font-family: wf_segoe-ui;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/bold/latest.eot"");
			src: local(""Segoe UI Bold""), local(""Segoe Bold""), local(""Segoe WP Bold""), local(""Segoe UI""), local(""Segoe""), local(""Segoe WP""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/bold/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/bold/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/bold/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/bold/latest.ttf"") format(""truetype"");
			font-weight: bold;
			font-style: normal;
		}

		@font-face {
			font-family: wf_segoe-ui_light;
			src: url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.eot"");
			src: local(""Segoe UI Light""), local(""Segoe Light""), local(""Segoe WP Light""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.eot?#iefix"") format(""embedded-opentype""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.woff2"") format(""woff2""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.woff"") format(""woff""), url(""//c.s-microsoft.com/static/fonts/segoe-ui/west-european/light/latest.ttf"") format(""truetype"");
			font-weight: normal;
			font-style: normal;
		}

		@font-face {
			font-family: 'bowtie';
			font-style: normal;
			font-weight: normal;
			src: url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Fonts/Icons/bowtie.eot);
			src: url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Fonts/Icons/bowtie.eot?iefix) format(""embedded-opentype""), url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Fonts/Icons/bowtie.woff) format(""woff""), url(https://cdn.vsassets.io/v/M204_20220530.3/_content/Fonts/Icons/bowtie.svg#bowtie) format(""svg"");
		}

		/* Verified Domain style begin */

		/* Verified Domain style end */

		/* top pub. style begin */


		/* top pub. style end */

		/*rating review style begin*/
		.item-details-control-root .ux-item-review-rating .ux-item-rating-control .rating-control .star {
			min-width: 16px;
			min-height: 16px;
			height: 12px;
			width: 12px;
			padding-top: 0px;
			margin: 0px 2px;
		}

		.item-details-control-root .ux-item-second-row-wrapper .ux-item-review-rating-wrapper {
			text-decoration: none;
			color: inherit;
			padding-top: 1px;
			padding-bottom: 3px;
		}

		.item-details-control-root .ux-item-second-row-wrapper .ux-item-review-rating-wrapper .ux-item-review-rating {
			display: inline-flex;
		}

		.item-details-control-root .ux-item-second-row-wrapper .ux-item-review-rating-wrapper .ux-item-review-rating .ux-item-rating-control {
			margin-left: 10px;
			top: 2px;
			position: relative;
		}

		.item-details-control-root .ux-item-second-row-wrapper .ux-item-review-rating-wrapper .ux-item-review-rating .ux-item-rating-count {
			margin-left: 5px;
			font-size: 16px;
			margin-top: -1px;
			margin-right: 8px;
		}

		/*rating review style end*/
		/*Spinner*/
		.loader-container {
			text-align: -webkit-center;
			text-align: -mozkit-center;
		}

		.loader {
			border: 3px solid #f3f3f3;
			border-radius: 50%;
			border-top: solid #808080;
			width: 30px;
			height: 30px;
			-webkit-animation: spin 2s linear infinite;
			animation: spin 2s linear infinite;
		}

		@-webkit-keyframes spin {
			0% {
				-webkit-transform: rotate(0deg);
			}

			100% {
				-webkit-transform: rotate(360deg);
			}
		}

		@keyframes spin {
			0% {
				transform: rotate(0deg);
			}

			100% {
				transform: rotate(360deg);
			}
		}

		.tooltip {
			position: relative;
			display: inline-block;
			border-bottom: 1px dotted black;
		}

		.tooltip .tooltiptext {
			visibility: hidden;
			width: 310px;
			background-color: black;
			color: white;
			text-align: center;
			border-radius: 6px;
			padding: 0px 0;
			position: absolute;
			z-index: 1;
			top: -1px;
			right: 100%;
		}

		.tooltip:hover .tooltiptext {
			visibility: visible;
		}

		.ms-Button-icon.bowtie-icon.bowtie-copy-to-clipboard.icon-64:before {
			content: ""\E8C8"";
			font-family: AzureDevOpsMDL2Assets;
		}
	</style>


	<script type=""text/javascript"" src=""https://wcpstatic.microsoft.com/mscc/lib/v2/wcp-consent.js""></script>


	<script type=""text/javascript"" nonce=""bU9YcLn/jZlYQZR8hZkkdg=="">
		function sue(em, s, l, c, eo){var w=window,p=""ue"";w[p]=w[p]||[];w[p].push({""em"":em,""s"":s,""l"":l,""c"":c,""eo"":eo});return false;}
            window.onerror = sue;
            
        isServerSideRendering = function () {
            try {
                return !(document !== undefined);
            }
            catch (e) {
                return true;
            }
         };

        redirectToNonSSR = function(area) {
            var fqn = document.getElementById(""FQN"").value;
            var galleryUrl = document.getElementById(""galleryUrl"").value;
            window.location.assign(window.location.origin + galleryUrl +""items?itemName="" + fqn + ""&ssr=false"" + area);
        }
        
        hideAndShowTabs = function(tabToShow) {
            var currentTab = document.getElementsByClassName(""selected-tab"");
            currentTab[0].style.display = ""none"";
            currentTab[0].classList.remove(""selected-tab"");

            var selectedTab =  document.getElementById(tabToShow);
            selectedTab.classList.add(""selected-tab"");
            selectedTab.style.display = ""block"";

            // Changing the button styling for the newly clicked button 
            var currentTabButton = document.getElementsByClassName(""selected"");
            currentTabButton[0].classList.remove(""selected"");

            var selectedTabButton =  document.getElementById(tabToShow.slice(0, -3));
            selectedTabButton.classList.add(""selected"");
        }

            if (!isServerSideRendering()) {
                document.addEventListener('DOMContentLoaded', function () {
                
                    document.getElementById(""overview"").addEventListener(""click"", function (e) {
                        hideAndShowTabs(""overviewTab"");
                    });
                    
                    document.getElementById(""qna"").addEventListener(""click"", function (e) {
                        redirectToNonSSR(""#qna"");
                    });
                    
                    document.getElementById(""rnr"").addEventListener(""click"", function (e) {
                        redirectToNonSSR(""#review-details"");
                    });
                    document.getElementById(""review-details"").addEventListener(""click"", function (e) {
                        redirectToNonSSR(""#review-details"");
                    });
                    

                    // add handler for search
                    document.getElementById(""Fragment_SearchBox"").addEventListener(""submit"", function (e) {
                        e.preventDefault();
                        var queryText = document.getElementById(""header-search-textbox"").value;
                        var galleryUrl = document.getElementById(""galleryUrl"").value;
                        var searchTarget = document.getElementById(""searchTarget"").value;
                        window.location = window.location.origin + galleryUrl + ""search?term="" + queryText + ""&target="" + searchTarget;
                    });
                    let acqBtn = document.getElementsByClassName(""install-button-container"");
                    if (acqBtn && acqBtn[0]) {
                        acqBtn[0].onclick = function (event) { var w = window, p = ""ciEvents""; w[p] = w[p] || []; w[p].push({ ""acquisition"": true, ""event"": event }); };
                    }

                

                });
            }
            document.addEventListener('DOMContentLoaded', function () {
                if (document.getElementById(""mgtConsentCookie"") != null) {
                    document.getElementById(""mgtConsentCookie"").addEventListener(""click"", function (e) {
                        console.log(""from aspx hit."");
                        manageConsent();
                    });
                }                
            });
	</script>
	<title>
		I Hate #Regions - Visual Studio Marketplace
	</title>
</head>
<a href=""#start-of-content"" class=""skip-main"">Skip to content</a>

<body class=""platform gallery gallery-page-item-details"">
	<div id=""cookie-banner""></div>
	<script nonce=""bU9YcLn/jZlYQZR8hZkkdg==""
		src=""/_static/tfs/M204_20220530.3/_scripts/TFS/min/Gallery/Client/Common/ManageCookieDrop.js""></script>


	<div class=""uxservices-header"" role=""banner"">
		<div class=""upperBandContent"">
			<a href=""/"" title=""|   Marketplace"" class=""left"">
				<img class=""vs-logo-header"" aria-label=""Visual Studio logo"" src=""https://cdn.vsassets.io/v/M204_20220530.3/_content/Header/vs-logo.png""/>
				<div id=""marketPlaceLogoLink"" class=""marketPlaceLogoLink marketplacetext-header""
					aria-label=""Marketplace logo"">| Marketplace</div>
			</a>
			<div class=""right"" style=""float: right;"">

				<div class=""signIn"">

					<a href=""https://app.vssps.visualstudio.com/_signin?realm=marketplace.visualstudio.com&amp;reply_to=https%3A%2F%2Fmarketplace.visualstudio.com%2Fitems%3FitemName%3DShanewho.IHateRegions&amp;redirect=1&amp;context=eyJodCI6MywiaGlkIjoiMjY2M2IxM2YtNTBlMy1hNjU1LWExNTktMjJmNmY0NzI1ZmFiIiwicXMiOnt9LCJyciI6IiIsInZoIjoiIiwiY3YiOiIiLCJjcyI6IiJ90&amp;workflowId=marketplace&amp;wt.mc_id=o~msft~marketplace~signIn#ctx=eyJTaWduSW5Db29raWVEb21haW5zIjpbImh0dHBzOi8vbG9naW4ud2luZG93cy5uZXQiLCJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20iXX01""
						class=""scarabLink"" style=""margin-left:23px;padding-top:1px;color:#fff;font-weight:400;"">
						Sign in
					</a>

				</div>

				<form id=""Fragment_SearchBox"" class=""header-search"" aria-label=""search"" role=""search""
					data-fragmentname=""SearchBox"">
					<input id=""header-search-textbox"" class=""header-search-textbox"" aria-label=""search"" max-length=""200"" type=""text"" autocomplete=""off"">
					<button id=""header-search-button"" class=""header-search-button"" aria-label=""search"" type=""submit""></button>
				</form>
			</div>
		</div>
	</div>
	<div class=""skiptarget"" id=""start-of-content""></div>
	<div id=""react_0HMIF3425P1DI"">
		<div data-reactroot="""">
			<div class=""item-details-control-root"">
				<div class=""breadcrumb"">
					<div class=""gallery-centered-content"">
						<span class=""bread-crumb-container""><a class=""member"" href=""/vs"">Visual Studio</a><span class=""separator"">&gt;</span><a
							class=""member""
							href=""/search?sortBy=Installs&amp;category=Tools&amp;target=VS"">Tools</a><span class=""separator"">&gt;</span><span class=""member"">I Hate #Regions</span></span>
					</div>
				</div>
				<div class=""main-content item-details-main-content"" id=""vss_1"">
					<div class=""item-details-control-root"">
						<div class=""ux-item-details"">
							<div class=""ux-section-banner"" id=""section-banner"">
								<div class=""ux-section-core gallery-centered-content"">
									<table role=""presentation"">
										<tbody>
											<tr>
												<td class=""item-img"" id=""vss_2"">
													<img class=""image-display"" alt="""" src=""https://shanewho.gallerycdn.vsassets.io/extensions/shanewho/ihateregions/1.4.1/1523844662971/Microsoft.VisualStudio.Services.Icons.Default"" style=""top:0.5px;visibility:visible""/></td>
												<td class=""item-header"">
													<div class=""item-header-content light"">
														<h1><span class=""ux-item-name"">I Hate #Regions</span></h1>
														<div class=""ux-item-second-row-wrapper"">
															<div class=""ux-item-publisher"">
																<h2 role=""presentation""><a
																		class=""ux-item-publisher-link item-banner-focussable-child-item""
																		href=""publishers/Shanewho""
																		aria-label=""More from Shanewho publisher""
																		style=""color:#000000"">Shanewho</a></h2>
															</div><span class=""divider""> | </span>
															<div class=""ux-item-rating"">
																<div class=""bowtie-icon bowtie-install""></div>
																<span class=""installs-text"" title=""The number of unique installations, not including updates.""> 50,193 installs</span>
															</div><a id=""review-details"" href=""#review-details""
																class=""ux-item-review-rating-wrapper""
																aria-label=""Average rating: 4.8 out of 5. Navigate to user reviews.""><span class=""ux-item-review-rating"" title=""Average rating: 4.8 out of 5""><span class=""divider""> | </span><span class=""ux-item-rating-control""><span class=""rating-control""><img class=""star full-star"" alt="""" aria-label=""1 star"" src=""https://cdn.vsassets.io/v/M204_20220530.3/_content/FullStar.svg""/><img class=""star full-star"" alt="""" aria-label=""2 star"" src=""https://cdn.vsassets.io/v/M204_20220530.3/_content/FullStar.svg""/><img class=""star full-star"" alt="""" aria-label=""3 star"" src=""https://cdn.vsassets.io/v/M204_20220530.3/_content/FullStar.svg""/><img class=""star full-star"" alt="""" aria-label=""4 star"" src=""https://cdn.vsassets.io/v/M204_20220530.3/_content/FullStar.svg""/><img class=""star full-star"" alt="""" aria-label=""5 star"" src=""https://cdn.vsassets.io/v/M204_20220530.3/_content/FullStar.svg""/></span></span><span class=""ux-item-rating-count""> (<span>119</span>)</span></span></a><span class=""divider""> | </span><span class=""item-price-category"">Free</span>
														</div>
														<div class=""ux-item-shortdesc"">Regions Suck. You need this.
														</div>
														<div class=""ux-item-action""><a class=""install-button-container""
																target=""_self"" tabIndex=""-1""
																href=""/_apis/public/gallery/publishers/Shanewho/vsextensions/IHateRegions/1.4.1/vspackage""
																rel=""noreferrer noopener nofollow""
																role=""presentation""><button class=""ux-button install"">Download</button></a>
															<div style=""display:none"">
																<input type=""text"" id=""FQN"" readonly="""" value=""Shanewho.IHateRegions""/><input type=""text"" id=""galleryUrl"" readonly="""" value=""/""/><input type=""text"" id=""searchTarget"" readonly="""" value=""VS&amp;vsVersion=vs2019""/></div>
															</div>
														</div>
												</td>
											</tr>
										</tbody>
									</table>
								</div>
							</div>
							<div class=""gallery-centered-content"">
								<div class=""ux-section-details"">
									<div class=""ms-Fabric ux-section-details-tabs"">
										<div class=""gallery-centered-content"">
											<div class=""tab"">
												<button id=""overview"" class=""selected"">Overview</button><button id=""qna"">Q &amp; A</button><button id=""rnr"">Rating &amp; Review</button>
											</div>
											<div class=""tabcontent"">
												<div id=""overviewTab"" class=""overview selected-tab"">
													<div class=""details-tab itemdetails"">
														<table class=""ux-section-details-table"" role=""presentation"">
															<tbody>
																<tr>
																	<td class=""ux-itemdetails-left"">
																		<div class=""itemDetails"">
																			<div class=""markdown"">
																				<h1
																					id=auto-expand-visual-studio-regions>
																					Auto-Expand Visual Studio Regions
																				</h1>
																				<h2
																					id=make-visual-studio-regions-suck-less>
																					Make Visual Studio #regions suck
																					less.</h2>
																				<p>Works with 2010, 2012, and 2013, 2015
																				</p>
																				<h2 id=automatically-turn-this>
																					Automatically turn this:</h2>
																				<p><img src=""https://shanewho.gallerycdn.vsassets.io/extensions/shanewho/ihateregions/1.4.1/1523844662971/84603/1/Regions1.png"" alt=Regions1></p>
																					<h2 id=into-this>Into this:</h2>
																					<p><img src=""https://shanewho.gallerycdn.vsassets.io/extensions/shanewho/ihateregions/1.4.1/1523844662971/69115/1/Regions2.png"" alt=Regions2></p>
																						<p>Automatically expand regions
																							when opening a file. Prevent
																							regions from being closed
																							while collapsing other code
																							blocks, and blend the region
																							text into the background,
																							making it less noticable.
																							See
																							Tools-&gt;Options-&gt;Disable
																							Regions for options.</p>
																			</div>
																		</div>
																	</td>
																	<td class=""ux-itemdetails-right""
																		role=""complementary"">
																		<div class=""screenshot-carousel-container"">
																		</div>
																		<div class=""loader-container"">
																			<div class=""loader""></div>
																		</div>
																		<div class=""itemDetails-right""></div>
																	</td>
																</tr>
															</tbody>
														</table>
													</div>
												</div>
											</div>
										</div>
									</div>
								</div>
							</div>
						</div>
					</div>
				</div>
				<div id=""gallery-footer"">
					<footer>
						<div id=""ux-footer"" class=""ltr"" role=""contentinfo"">
							<div id=""baseFooter"">
								<div id=""Fragment_BaseFooterLinks"">
									<div class=""linkList"">
										<ul class=""links horizontal"">
											<li><a href=""https://www.visualstudio.com/support/support-overview-vs""
													data-mscc-ic=""false"">Contact us</a></li>
											<li><a href=""https://careers.microsoft.com/"" data-mscc-ic=""false"">Jobs</a>
											</li>
											<li><a href=""https://go.microsoft.com/fwlink/?LinkID=264782""
													data-mscc-ic=""false"">Privacy</a></li>
											<li><a href=""https://aka.ms/vsmarketplace-ToU"" data-mscc-ic=""false"">Terms of
													use</a></li>
											<li><a href=""https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/EN-US.aspx""
													data-mscc-ic=""false"">Trademarks</a></li>
										</ul>
									</div>
								</div>
								<div id=""rightBaseFooter"">©
									<!-- --> 2022 Microsoft<span class=""microsoftLogo"" title=""Microsoft""></span></div>
								<div class=""clear""></div>
							</div>
						</div>
					</footer>
				</div>
			</div>
		</div>
	</div>

	<!-- Google Tag Manager -->
	<noscript>
		<iframe src=""//www.googletagmanager.com/ns.html?id=GTM-WT9XBK"" height=""0"" width=""0""
			style=""display: none; visibility: hidden""></iframe>
	</noscript>
	<script type=""text/javascript"" nonce=""bU9YcLn/jZlYQZR8hZkkdg=="">
		(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
        new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName('body')[0],
        j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.defer=true;j.src=
        '//www.googletagmanager.com/gtm.js?id='+i+dl;f.insertBefore(j,f.lastChild);
        })(window,document,'script','dataLayer','GTM-WT9XBK');
	</script>
	<!-- End Google Tag Manager -->

	<div class=""render-time"">
		<script class=""server-side-render"" defer=""defer"" type=""application/json"">
			2
		</script>
	</div>
	<div class=""rhs-content"">
		<script class=""jiContent"" defer=""defer"" type=""application/json"">
			{""GitHubLink"":"""",""ReleaseDateString"":""Thu, 22 Mar 2012 03:30:47 GMT"",""LastUpdatedDateString"":""Mon, 16 Apr 2018 02:10:49 GMT"",""GalleryUrl"":""/"",""Categories"":[""Tools"",""Tools/Coding"",""Tools/Programming Languages""],""Tags"":[""regions #regions #region region c# vb vb.net""],""ExtensionProperties"":{""Microsoft.VisualStudio.Services.EnableMarketplaceQnA"":""True"",""Microsoft.VisualStudio.Services.Payload.FileName"":""69113/8/DisableRegions.vsix""},""Resources"":{""LicenseText"":""License"",""ChangelogText"":"""",""PublisherName"":""Shanewho"",""ExtensionName"":""IHateRegions"",""Version"":""1.4.1""},""MoreInfo"":{""VersionValue"":""1.4.1"",""PublisherValue"":""Shanewho"",""UniqueIdentifierValue"":""Shanewho.IHateRegions"",""TwitterShareContents"":""Just%20discovered%20this%20on%20the%20%23VSMarketplace%3A%20https%3A%2F%2Fmarketplace.visualstudio.com%2Fitems%3FitemName%3DShanewho.IHateRegions"",""EmailShareContents"":""Hi%2C%20Just%20discovered%20this%20extension%20on%20the%20%23VSMarketplace%20that%20may%20be%20of%20interest%20to%20you.%20Check%20it%20out%20%40%20https%3A%2F%2Fmarketplace.visualstudio.com%2Fitems%3FitemName%3DShanewho.IHateRegions%20%21"",""EmailShareSubject"":""Check%20out%20-%20I%20Hate%20%23Regions%20for%20Visual%20Studio%20Code"",""IsPublic"":true},""ResourcesPath"":""https://cdn.vsassets.io/v/M204_20220530.3/_content/"",""AssetUri"":""https://shanewho.gallerycdn.vsassets.io/extensions/shanewho/ihateregions/1.4.1/1523844662971"",""VsixManifestAssetType"":""Microsoft.VisualStudio.Services.VsixManifest"",""StaticResourceVersion"":""M204_20220530.3"",""AfdIdentifier"":""Ref A: 7186980C0DB04D3C9AC09DA9E9E21BAA Ref B: BY3EDGE0314 Ref C: 2022-06-22T04:32:31Z"",""VsixId"":""9387998a-9a53-4523-9ce9-9879f451e702"",""WorksWith"":[""Visual Studio 2010, 2012, 2013, 2015""],""ItemType"":4,""IsMDPruned"":false,""PrunedMDLength"":0,""OverviewMDLength"":476,""IsRHSAsyncComponentsEnabled"":true,""OfferDetails"":null,""IsDetailsTabsEnabled"":false,""ShowVersionHistory"":false,""IsSeeMoreButtonOnVersionHistoryTab"":false,""IsReferralLinkRedirectionWarningPopupEnabled"":true,""Versions"":[{""version"":""1.4.1"",""lastUpdated"":""Mon, 16 Apr 2018 02:11:02 GMT"",""targetPlatform"":null}],""IsCSRFeatureEnabled"":false,""TargetPlatforms"":null}
		</script>
	</div>
	<div class=""csp-user"">
		<script class=""is-csp-user"" defer=""defer"" type=""application/json"">
			null
		</script>
	</div>


	<script type=""text/javascript"" nonce=""bU9YcLn/jZlYQZR8hZkkdg=="">
		if (document) {
            try {
                if (window.performance && window.performance.timing) {
                    window[""marketplaceRenderTime""] = Date.now() - window.performance.timing.navigationStart;
                }
            }
            catch(e){ }
        }
	</script>
	<script type=""text/javascript"" nonce=""bU9YcLn/jZlYQZR8hZkkdg=="">
		var __vssPageContext = {""webContext"":{""host"":{""id"":""2663b13f-50e3-a655-a159-22f6f4725fab"",""name"":""TEAM FOUNDATION"",""uri"":""https://marketplace.visualstudio.com/"",""relativeUri"":""/"",""hostType"":""deployment"",""scheme"":""https"",""authority"":""marketplace.visualstudio.com""}},""moduleLoaderConfig"":{""baseUrl"":""https://cdn.vsassets.io/v/M204_20220530.3/_scripts/TFS/min/"",""paths"":{""ContentRendering/Resources"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""Engagement/Resources"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""Gallery/Scripts/Gallery/Resources"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""VSSPreview/Resources"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""VSS/Resources"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""Charts/Resources"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""highcharts"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts.v9.0.1"",""highcharts/highcharts-more"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-more.v9.0.1"",""highcharts/modules/accessibility"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-accessibility.v9.0.1"",""highcharts/modules/funnel"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-funnel.v9.0.1"",""highcharts/modules/heatmap"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-heatmap.v9.0.1""},""map"":{""*"":{""office-ui-fabric-react/lib"":""OfficeFabric""}},""contributionPaths"":{""VSS"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/VSS"",""pathType"":""default""},""VSS/Resources"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""pathType"":""resource""},""q"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/q"",""pathType"":""default""},""knockout"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/knockout"",""pathType"":""default""},""mousetrap"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/mousetrap"",""pathType"":""default""},""mustache"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/mustache"",""pathType"":""default""},""react"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/react.15.3"",""pathType"":""default""},""react-dom"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/react-dom.15.3"",""pathType"":""default""},""react-transition-group"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/react-transition-group.15.3"",""pathType"":""default""},""jQueryUI"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/jQueryUI"",""pathType"":""default""},""jquery"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/jquery"",""pathType"":""default""},""OfficeFabric"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/OfficeFabric"",""pathType"":""default""},""tslib"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/tslib"",""pathType"":""default""},""@uifabric"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/@uifabric"",""pathType"":""default""},""VSSUI"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/VSSUI"",""pathType"":""default""},""ContentRendering"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/ContentRendering"",""pathType"":""default""},""ContentRendering/Resources"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""pathType"":""resource""},""Charts"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/Charts"",""pathType"":""default""},""Charts/Resources"":{""value"":""/_static/tfs/M204_20220530.3/_scripts/TFS/min/en-US"",""pathType"":""resource""},""highcharts"":{""value"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts.v9.0.1"",""pathType"":""thirdParty""},""highcharts/highcharts-more"":{""value"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-more.v9.0.1"",""pathType"":""thirdParty""},""highcharts/modules/accessibility"":{""value"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-accessibility.v9.0.1"",""pathType"":""thirdParty""},""highcharts/modules/funnel"":{""value"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-funnel.v9.0.1"",""pathType"":""thirdParty""},""highcharts/modules/heatmap"":{""value"":""https://cdn.vsassets.io/3rdParty/_scripts/highcharts-heatmap.v9.0.1"",""pathType"":""thirdParty""}},""shim"":{""jquery"":{""deps"":[],""exports"":""jQuery""}},""waitSeconds"":30},""coreReferences"":{""stylesheets"":[{""url"":""/_static/tfs/M204_20220530.3/_cssbundles/Default/vss-bundle-ext-core-css-vq-0UMAOWfv_vTtRevTd08i3RY1zK3Kepo5uGybSV2q8="",""highContrastUrl"":null,""isCoreStylesheet"":true}],""scripts"":[{""identifier"":""JQuery"",""url"":""https://cdn.vsassets.io/3rdParty/_scripts/jquery-2.2.4.min.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true},{""identifier"":""JQueryXDomain"",""url"":""https://cdn.vsassets.io/3rdParty/_scripts/jquery.xdomainrequest.min.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true},{""identifier"":""Promise"",""url"":""https://cdn.vsassets.io/v/M204_20220530.3/_scripts/TFS/min/promise.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true},{""identifier"":""GlobalScripts"",""url"":""https://cdn.vsassets.io/v/M204_20220530.3/_scripts/TFS/min/global-scripts.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true},{""identifier"":""LoaderFixes"",""url"":""https://cdn.vsassets.io/v/M204_20220530.3/_scripts/TFS/pre-loader-shim.min.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":false},{""identifier"":""AMDLoader"",""url"":""https://cdn.vsassets.io/3rdParty/_scripts/require.min.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true},{""identifier"":""LoaderFixes"",""url"":""https://cdn.vsassets.io/v/M204_20220530.3/_scripts/TFS/post-loader-shim.min.js"",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":false}],""coreScriptsBundle"":{""identifier"":""CoreBundle"",""url"":""/_public/_Bundling/Content?bundle=vss-bundle-basejs-v9GpWWBnsWqhM23ijhK2HfAqLowTXGUqZLDRsBCZbkfY="",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true},""extensionCoreReferences"":{""identifier"":""CoreBundle"",""url"":""https://cdn.vsassets.io/bundles/vss-bundle-ext-core-vByfwuf1w9fVtoVIOW-eFpGj0uMs8zxMe9MM5eVQIeGU="",""fallbackUrl"":null,""fallbackCondition"":null,""isCoreModule"":true}},""webAccessConfiguration"":{""isHosted"":true,""paths"":{""rootPath"":""/"",""staticContentRootPath"":""/"",""staticContentVersion"":""M204_20220530.3"",""resourcesPath"":""https://cdn.vsassets.io/v/M204_20220530.3/_content/"",""staticRootTfs"":""https://cdn.vsassets.io/v/M204_20220530.3/"",""cdnFallbackStaticRootTfs"":""/_static/tfs/M204_20220530.3/"",""staticRoot3rdParty"":""https://cdn.vsassets.io/3rdParty/""},""api"":{""webApiVersion"":""1"",""areaPrefix"":""_"",""controllerPrefix"":""_""},""mailSettings"":{""enabled"":false},""registryItems"":{}},""microsoftAjaxConfig"":{""cultureInfo"":{""name"":""en-US"",""numberFormat"":{""CurrencyDecimalDigits"":2,""CurrencyDecimalSeparator"":""."",""IsReadOnly"":true,""CurrencyGroupSizes"":[3],""NumberGroupSizes"":[3],""PercentGroupSizes"":[3],""CurrencyGroupSeparator"":"","",""CurrencySymbol"":""$"",""NaNSymbol"":""NaN"",""CurrencyNegativePattern"":0,""NumberNegativePattern"":1,""PercentPositivePattern"":1,""PercentNegativePattern"":1,""NegativeInfinitySymbol"":""-∞"",""NegativeSign"":""-"",""NumberDecimalDigits"":2,""NumberDecimalSeparator"":""."",""NumberGroupSeparator"":"","",""CurrencyPositivePattern"":0,""PositiveInfinitySymbol"":""∞"",""PositiveSign"":""+"",""PercentDecimalDigits"":2,""PercentDecimalSeparator"":""."",""PercentGroupSeparator"":"","",""PercentSymbol"":""%"",""PerMilleSymbol"":""‰"",""NativeDigits"":[""0"",""1"",""2"",""3"",""4"",""5"",""6"",""7"",""8"",""9""],""DigitSubstitution"":1},""dateTimeFormat"":{""AMDesignator"":""AM"",""Calendar"":{""MinSupportedDateTime"":""0001-01-01T00:00:00"",""MaxSupportedDateTime"":""9999-12-31T23:59:59.9999999"",""AlgorithmType"":1,""CalendarType"":1,""Eras"":[1],""TwoDigitYearMax"":2029,""IsReadOnly"":true},""DateSeparator"":""/"",""FirstDayOfWeek"":0,""CalendarWeekRule"":0,""FullDateTimePattern"":""dddd, MMMM d, yyyy h:mm:ss tt"",""LongDatePattern"":""dddd, MMMM d, yyyy"",""LongTimePattern"":""h:mm:ss tt"",""MonthDayPattern"":""MMMM d"",""PMDesignator"":""PM"",""RFC1123Pattern"":""ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"",""ShortDatePattern"":""M/d/yyyy"",""ShortTimePattern"":""h:mm tt"",""SortableDateTimePattern"":""yyyy'-'MM'-'dd'T'HH':'mm':'ss"",""TimeSeparator"":"":"",""UniversalSortableDateTimePattern"":""yyyy'-'MM'-'dd HH':'mm':'ss'Z'"",""YearMonthPattern"":""MMMM yyyy"",""AbbreviatedDayNames"":[""Sun"",""Mon"",""Tue"",""Wed"",""Thu"",""Fri"",""Sat""],""ShortestDayNames"":[""Su"",""Mo"",""Tu"",""We"",""Th"",""Fr"",""Sa""],""DayNames"":[""Sunday"",""Monday"",""Tuesday"",""Wednesday"",""Thursday"",""Friday"",""Saturday""],""AbbreviatedMonthNames"":[""Jan"",""Feb"",""Mar"",""Apr"",""May"",""Jun"",""Jul"",""Aug"",""Sep"",""Oct"",""Nov"",""Dec"",""""],""MonthNames"":[""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December"",""""],""IsReadOnly"":true,""NativeCalendarName"":""Gregorian Calendar"",""AbbreviatedMonthGenitiveNames"":[""Jan"",""Feb"",""Mar"",""Apr"",""May"",""Jun"",""Jul"",""Aug"",""Sep"",""Oct"",""Nov"",""Dec"",""""],""MonthGenitiveNames"":[""January"",""February"",""March"",""April"",""May"",""June"",""July"",""August"",""September"",""October"",""November"",""December"",""""]},""numberShortForm"":{""QuantitySymbols"":[""K"",""M"",""B""],""NumberGroupSize"":1000,""ThousandSymbol"":""K""},""eras"":null}},""timeZonesConfiguration"":{},""featureAvailability"":{""featureStates"":{""VisualStudio.Services.Contribution.EnableOnPremUnsecureBrowsers"":false,""VisualStudio.Service.WebPlatform.ClientErrorReporting"":true,""Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri"":false,""VisualStudio.Services.WebAccess.SubresourceIntegrity"":false,""VisualStudio.Services.IdentityPicker.ReactProfileCard"":true}},""appInsightsConfiguration"":{""enabled"":false,""instrumentationKey"":""00000000-0000-0000-0000-000000000000"",""insightsScriptUrl"":null},""diagnostics"":{""sessionId"":""c5a02770-7f66-4ca2-9007-ecbe11ecee93"",""activityId"":""c5a02770-7f66-4ca2-9007-ecbe11ecee93"",""bundlingEnabled"":true,""cdnAvailable"":true,""cdnEnabled"":true,""webPlatformVersion"":""M204"",""serviceVersion"":""Dev19.M204.1 (build: AzureDevOps_M204_20220530.3)""},""navigation"":{""topMostLevel"":""deployment"",""area"":"""",""currentController"":""Gallery"",""currentAction"":""Details"",""commandName"":""Gallery.Details"",""routeId"":""LegacyWebAccessRoute"",""routeValues"":{""controller"":""Gallery"",""action"":""Details"",""serviceHost"":""2663b13f-50e3-a655-a159-22f6f4725fab (TEAM FOUNDATION)""}},""globalization"":{""explicitTheme"":"""",""theme"":""Default"",""culture"":""en-US"",""timezoneOffset"":0,""timeZoneId"":""UTC""},""serviceInstanceId"":""00000029-0000-8888-8000-000000000000"",""hubsContext"":{},""serviceLocations"":{""locations"":{""951917ac-a960-4999-8464-e3f0aa25b381"":{""Application"":""https://app.vssps.visualstudio.com/"",""Deployment"":""https://app.vssps.visualstudio.com/""}}}};
	</script>
	<script defer=""true"" data-bundlelength=""125076"" data-bundlename=""basejs"" nonce=""bU9YcLn/jZlYQZR8hZkkdg==""
		src=""https://cdn.vsassets.io/bundles/vss-bundle-basejs-v9GpWWBnsWqhM23ijhK2HfAqLowTXGUqZLDRsBCZbkfY=""
		type=""text/javascript""></script>
	<script defer=""true"" data-bundlelength=""214554"" data-bundlename=""common"" data-includedscripts=""VSS/Bundling""
		nonce=""bU9YcLn/jZlYQZR8hZkkdg==""
		src=""https://cdn.vsassets.io/bundles/vss-bundle-common-vCPXDUVxkTJ0MXyp5hgbMfu7VPH4denaIDWitMDNBYvw=""
		type=""text/javascript""></script>
	<script defer=""true"" data-bundlelength=""2179879"" data-bundlename=""view""
		data-includedscripts=""Gallery/Client/Pages/VSSItemDetailsSSR/VSSItemDetailsSSRAsync""
		nonce=""bU9YcLn/jZlYQZR8hZkkdg==""
		src=""https://cdn.vsassets.io/bundles/vss-bundle-view-vJEfFSOEYz6M-LeKMSpOc-ZnUF_zJqpls98vRQVsZ1-s=""
		type=""text/javascript""></script>
	<script defer=true nonce=""bU9YcLn/jZlYQZR8hZkkdg==""
		src=""/_static/tfs/M204_20220530.3/_scripts/TFS/min/Gallery/Client/Pages/VSSItemDetailsSSR/SSRModuleWrappers.js"">
	</script>
</body>

</html>
";
        }
    }
}
