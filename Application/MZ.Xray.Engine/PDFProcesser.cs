using MigraDoc.Rendering;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Collections.Generic;
using MZ.Util;
using MZ.Domain.Entities;
using MigraDocStyle = MigraDoc.DocumentObjectModel.Style;
using MigraDocColors = MigraDoc.DocumentObjectModel.Colors;

namespace MZ.Xray.Engine
{
    /// <summary>
    /// Xray 이미지 및 객체 감지 통계 기반의 PDF 리포트 클래스
    /// </summary>
    public class PDFProcesser
    {
        public DateTime? StartSelectedDate { get; set; }
        public DateTime? EndSelectedDate { get; set; }
        
        public string Username { get; set; }
        private string TempPath { get; set; } = Path.GetTempPath();

        public PDFProcesser()
        {

        }

        /// <summary>
        /// PDF 리포트를 생성
        /// - 차트 이미지를 추출하여 보고서 표지, 목차, 통계, 이미지 섹션 등 포함
        /// - 최종 PDF 저장 및 임시 이미지 파일 삭제까지 처리
        /// </summary>
        public void Make(object framework, string outputPath, string objectDetectionChartName, string imageFileChartName, ICollection<ImageEntity> images)
        {
            //framework 에서 이미지 불러옴
            var objectDetectChart = MZFramework.FindChildByName(framework as DependencyObject, objectDetectionChartName);
            var imageFileChart = MZFramework.FindChildByName(framework as DependencyObject, imageFileChartName);

            (_, string objectDetectImagePath) = SaveChartImage(objectDetectChart, Path.Combine(TempPath, "ObjDetectChart.png"));
            (_, string imageFileImagePath) = SaveChartImage(imageFileChart, Path.Combine(TempPath, "ImgFileChart.png"));

            Document document = new();
            //document 설정
            SetDocument(document);

            //여백 계산
            Unit availableWidth = document.DefaultPageSetup.PageWidth - document.DefaultPageSetup.LeftMargin - document.DefaultPageSetup.RightMargin;

            //매인 커버
            AddSectionCover(document, availableWidth);

            //목차
            AddSectionIndex(document, availableWidth);

            //디택션 
            AddSectionObjectDetections(document, objectDetectImagePath, availableWidth, images);

            //이미지 파일
            AddSectionImageFiles(document, imageFileImagePath, availableWidth, images);

            //pdf 저장
            Rendering(document, outputPath);

            //파일 제거
            MZIO.TryDeleteFile(objectDetectImagePath);
            MZIO.TryDeleteFile(imageFileImagePath);
        }
        /// <summary>
        /// 전역 스타일, 폰트, 문서 속성 설정
        /// </summary>
        public void SetDocument(Document document)
        {
            GlobalFontSettings.FontResolver = new FontResolver();

            document.Info.Title = "Detection Report";
            document.Info.Subject = "Subject";
            document.Info.Author = "Author";

            MigraDocStyle style = document.Styles["Normal"];
            style.Font.Name = "맑은 고딕";
            style.Font.Size = 11;
            style.ParagraphFormat.SpaceAfter = Unit.FromCentimeter(0.5);

            MigraDocStyle heading1 = document.Styles["Heading1"];
            heading1.Font.Name = "맑은 고딕";
            heading1.Font.Size = 16;
            heading1.Font.Bold = true;
            heading1.ParagraphFormat.SpaceBefore = Unit.FromCentimeter(0.5);
            heading1.ParagraphFormat.SpaceAfter = Unit.FromCentimeter(0.5);

        }
        /// <summary>
        /// PDF 리포트 표지(커버) 페이지
        /// </summary>
        public void AddSectionCover(Document document, Unit availableWidth)
        {
            Section section = document.AddSection();
            AddHeaderFooter(document, section);

            Paragraph title = section.AddParagraph(document.Info.Title, "Heading1");
            title.Format.Alignment = ParagraphAlignment.Center;
            title.Format.Font.Size = 24;
            title.Format.SpaceAfter = Unit.FromCentimeter(1);

            Paragraph subtitle = section.AddParagraph("Comprehensive Report", "Heading3");
            subtitle.Format.Alignment = ParagraphAlignment.Center;
            subtitle.Format.Font.Size = 12;
            subtitle.Format.SpaceAfter = Unit.FromCentimeter(1.5);

            string periodText = "Search Date: ";
            if (StartSelectedDate.HasValue && EndSelectedDate.HasValue)
            {
                periodText += $"{StartSelectedDate.Value:yyyy-MM-dd HH:mm:ss} ~ {EndSelectedDate.Value:yyyy-MM-dd HH:mm:ss}";
            }
            Paragraph periodPara = section.AddParagraph(periodText);
            periodPara.Format.Alignment = ParagraphAlignment.Center;
            periodPara.Format.Font.Size = 12;

            string iconImagePath = "./Assets/Images/icon.png";
            var iconImage = section.AddImage(iconImagePath);
            iconImage.LockAspectRatio = true;
            iconImage.Width = Unit.FromCentimeter(5);
            iconImage.Left = ShapePosition.Center;
            iconImage.Top = Unit.FromCentimeter(1);

            TextFrame madeByFrame = section.AddTextFrame();
            madeByFrame.Height = Unit.FromCentimeter(1);
            madeByFrame.Width = availableWidth;
            madeByFrame.Left = ShapePosition.Center;
            madeByFrame.RelativeVertical = RelativeVertical.Page;
            madeByFrame.RelativeHorizontal = RelativeHorizontal.Page;
            madeByFrame.Top = Unit.FromCentimeter(27);

            Paragraph madeByPara = madeByFrame.AddParagraph($"Export by {Username}");
            madeByPara.Format.Alignment = ParagraphAlignment.Center;
            madeByPara.Format.Font.Size = 8;
            madeByPara.Format.Font.Color = MigraDocColors.Gray;
        }
        /// <summary>
        /// PDF 리포트 목차 페이지
        /// </summary>
        public void AddSectionIndex(Document document, Unit availableWidth)
        {

            Section section = document.AddSection();
            AddHeaderFooter(document, section);
            Paragraph title = section.AddParagraph("Index", "Heading1");
            title.Format.Alignment = ParagraphAlignment.Center;
            title.Format.SpaceAfter = Unit.FromCentimeter(1);

            IndexEntryParagraph(section, availableWidth, "Object Detections");
            IndexEntryParagraph(section, availableWidth, "Image Files");
            
        }

        /// <summary>
        /// 목차 한 항목을 표 형태로 추가
        /// </summary>
        public void IndexEntryParagraph(Section section, Unit availableWidth, string title)
        {
            Paragraph entry = section.AddParagraph();
            entry.Format.Font.Size = 12;
            entry.Format.TabStops.Clear();
            entry.Format.TabStops.AddTabStop(availableWidth, TabAlignment.Right, TabLeader.Dots);
            entry.AddText($"{title}");
            entry.AddTab();
            entry.AddPageRefField(title);
        }
        /// <summary>
        /// Object Detection(객체 감지 통계) 섹션 및 차트, 표 삽입
        /// </summary>
        public void AddSectionObjectDetections(Document document, string imagePath, Unit availableWidth, ICollection<ImageEntity> images, string titleName = "Object Detections")
        {
            Section section = document.AddSection();
            AddHeaderFooter(document, section);

            // 섹션 제목
            Paragraph title = section.AddParagraph(titleName, "Heading1");
            title.Format.SpaceBefore = Unit.FromCentimeter(1);
            title.Format.SpaceAfter = Unit.FromCentimeter(1);
            title.Format.Alignment = ParagraphAlignment.Center;
            title.AddBookmark(titleName);

            // 페이지 너비 계산
            double pageWidth = document.DefaultPageSetup.PageWidth.Centimeter;
            double leftMargin = document.DefaultPageSetup.LeftMargin.Centimeter;
            double rightMargin = document.DefaultPageSetup.RightMargin.Centimeter;
            double availableWidthCm = pageWidth - leftMargin - rightMargin;

            // 이미지 너비
            double desiredChartWidth = 20;
            double finalChartWidth = Math.Min(desiredChartWidth, availableWidthCm);

            // 이미지
            var image = section.AddImage(imagePath);
            image.LockAspectRatio = true;
            image.Width = Unit.FromCentimeter(finalChartWidth);
            image.Left = ShapePosition.Center;

            // 이미지 캡션
            Paragraph imageCaption = section.AddParagraph($"[ Figure 1: {titleName} ]", "Caption");
            imageCaption.Format.Font.Size = 10;
            imageCaption.Format.Font.Color = MigraDocColors.Black;
            imageCaption.Format.SpaceBefore = Unit.FromCentimeter(0.2);
            imageCaption.Format.Alignment = ParagraphAlignment.Center;

            // 이미지와 표 사이에 여백
            Paragraph gapParagraph = section.AddParagraph();
            gapParagraph.Format.SpaceBefore = Unit.FromCentimeter(0.5);

            // 표 생성 및 스타일
            Table table = section.AddTable();
            table.Borders.Width = 0.75;
            table.Borders.Color = MigraDocColors.Gray;
            table.Format.Alignment = ParagraphAlignment.Center;

            int columnsCount = 4;
            double colWidth1 = availableWidth.Centimeter / columnsCount;
            for (int i = 0; i < columnsCount; i++)
            {
                Column col = table.AddColumn($"{colWidth1:F2}cm");
                col.Format.Alignment = ParagraphAlignment.Center;
            }

            // 헤더
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = MigraDocColors.LightGray;
            headerRow.Cells[0].AddParagraph("Name");
            headerRow.Cells[1].AddParagraph("Count");
            headerRow.Cells[2].AddParagraph("Percentage");
            headerRow.Cells[3].AddParagraph("Color");
            headerRow.Format.Font.Bold = true;
            headerRow.Format.Font.Size = 10;

            // 데이터
            var model = images.SelectMany(image => image.ObjectDetections).ToList();
            var groupedDetections = model
                .GroupBy(det => det.Name)
                .Select(g => new { Name = g.Key, Count = g.Count(), g.First().Color })
                .OrderByDescending(x => x.Count)
                .ToList();


            double totalDetectionCount = groupedDetections.Sum(g => g.Count);
            foreach (var group in groupedDetections)
            {
                double percentage = totalDetectionCount > 0 ? (group.Count / totalDetectionCount) * 100 : 0;
                Row row = table.AddRow();
                row.Format.Font.Size = 10;
                row.Cells[0].AddParagraph(group.Name);
                row.Cells[1].AddParagraph(group.Count.ToString());
                row.Cells[2].AddParagraph(percentage.ToString("F2") + " %");
                Paragraph colorParagraph = row.Cells[3].AddParagraph(group.Color);
                try
                {
                    MigraDoc.DocumentObjectModel.Color cellColor = HexToColor(group.Color);
                    colorParagraph.Format.Font.Color = cellColor;
                }
                catch
                {
                    colorParagraph.Format.Font.Color = MigraDocColors.Black;
                }
            }
            Row totalRow = table.AddRow();
            totalRow.Format.Font.Bold = true;
            totalRow.Format.Font.Size = 12;
            totalRow.Cells[0].AddParagraph("Total");
            totalRow.Cells[1].AddParagraph(totalDetectionCount.ToString());
            totalRow.Cells[2].AddParagraph("100 %");
            totalRow.Cells[3].AddParagraph(" ");

            foreach (Row row in table.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    foreach (var element in cell.Elements)
                    {
                        if (element is Paragraph paragraph)
                        {
                            paragraph.Format.SpaceBefore = Unit.FromCentimeter(0.1);
                            paragraph.Format.SpaceAfter = Unit.FromCentimeter(0.1);
                        }
                    }
                }
            }

            // 표 캡션
            Paragraph tableCaption = section.AddParagraph($"[ Table 1: {titleName} ]", "Caption");
            tableCaption.Format.Font.Size = 10;
            tableCaption.Format.Font.Color = MigraDocColors.Black;
            tableCaption.Format.SpaceBefore = Unit.FromCentimeter(0.2);
            tableCaption.Format.Alignment = ParagraphAlignment.Center;
        }
        /// <summary>
        /// 이미지 파일 통계 섹션 및 차트, 표 삽입
        /// </summary>
        public void AddSectionImageFiles(Document document, string imagePath, Unit availableWidth, ICollection<ImageEntity> images, string titleName = "Image Files")
        {
            Section section = document.AddSection();
            AddHeaderFooter(document, section);

            // 제목
            Paragraph title = section.AddParagraph($"{titleName}", "Heading1");
            title.Format.SpaceBefore = Unit.FromCentimeter(0.5);
            title.Format.SpaceAfter = Unit.FromCentimeter(0.5);
            title.Format.Alignment = ParagraphAlignment.Center;
            title.AddBookmark(titleName);

            // 이미지
            var image = section.AddImage(imagePath);
            image.LockAspectRatio = true;
            image.Width = Unit.FromCentimeter(15);

            // 이미지 캡션
            Paragraph imageCaption = section.AddParagraph($"[ Figure 2: {titleName} ]", "Caption");
            imageCaption.Format.Font.Size = 10;
            imageCaption.Format.Font.Color = MigraDocColors.Black;
            imageCaption.Format.SpaceBefore = Unit.FromCentimeter(0.2);
            imageCaption.Format.Alignment = ParagraphAlignment.Center;

            // 여백
            Paragraph gapParagraph = section.AddParagraph();
            gapParagraph.Format.SpaceBefore = Unit.FromCentimeter(0.5);

            // 표 생성 및 스타일 설정
            Table table = section.AddTable();
            table.Borders.Width = 0.75;
            table.Borders.Color = MigraDocColors.Gray;
            table.Format.Alignment = ParagraphAlignment.Center;

            int columnsCount = 2;
            double colWidth = availableWidth.Centimeter / columnsCount;
            for (int i = 0; i < columnsCount; i++)
            {
                Column col = table.AddColumn($"{colWidth:F2}cm");
                col.Format.Alignment = ParagraphAlignment.Center;
            }

            // 헤더
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = MigraDocColors.LightGray;
            headerRow.Cells[0].AddParagraph("Date");
            headerRow.Cells[1].AddParagraph("Image Count");
            headerRow.Format.Font.Bold = true;
            headerRow.Format.Font.Size = 10;

            // 데이터
            var groupedImages = images
                .GroupBy(image => image.CreateDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            double totalImageCount = groupedImages.Sum(g => g.Count);
            foreach (var group in groupedImages)
            {
                Row row = table.AddRow();
                row.Format.Font.Size = 10;
                row.Cells[0].AddParagraph(group.Date.ToString("yyyy-MM-dd"));
                row.Cells[1].AddParagraph(group.Count.ToString());
            }

            Row totalRow = table.AddRow();
            totalRow.Format.Font.Bold = true;
            totalRow.Format.Font.Size = 12;
            totalRow.Cells[0].AddParagraph("Total");
            totalRow.Cells[1].AddParagraph(totalImageCount.ToString());

            // 각 셀 내부 여백 설정
            foreach (Row row in table.Rows)
            {
                foreach (Cell cell in row.Cells)
                {
                    foreach (var element in cell.Elements)
                    {
                        if (element is Paragraph paragraph)
                        {
                            paragraph.Format.SpaceBefore = Unit.FromCentimeter(0.1);
                            paragraph.Format.SpaceAfter = Unit.FromCentimeter(0.1);
                        }
                    }
                }
            }

            // 표 캡션
            Paragraph tableCaption2 = section.AddParagraph($"[ Table 2: {titleName} ]", "Caption");
            tableCaption2.Format.Font.Size = 10;
            tableCaption2.Format.Font.Color = MigraDocColors.Black;
            tableCaption2.Format.SpaceBefore = Unit.FromCentimeter(0.2);
            tableCaption2.Format.Alignment = ParagraphAlignment.Center;

        }

        /// <summary>
        /// 각 섹션에 Header/Footer 추가
        /// </summary>
        public void AddHeaderFooter(Document document, Section section)
        {
            //header
            HeaderFooter header = section.Headers.Primary;
            header.Elements.Clear();
            Paragraph headerParagraph = header.AddParagraph();
            headerParagraph.AddFormattedText(document.Info.Title, TextFormat.Bold);
            headerParagraph.AddTab();
            headerParagraph.AddText("Date : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            headerParagraph.Format.Font.Size = 9;
            headerParagraph.Format.Alignment = ParagraphAlignment.Center;

            //footer
            HeaderFooter footer = section.Footers.Primary;
            footer.Elements.Clear();
            Paragraph footerParagraph = footer.AddParagraph();
            footerParagraph.AddText("Page ");
            footerParagraph.AddPageField();
            footerParagraph.AddText(" of ");
            footerParagraph.AddNumPagesField();
            footerParagraph.Format.Font.Size = 9;
            footerParagraph.Format.Alignment = ParagraphAlignment.Center;
        }

        /// <summary>
        /// PDF 파일로 렌더링 및 저장
        /// PDF 페이지에 외곽선 테두리 추가
        /// </summary>
        public void Rendering(Document document, string path)
        {

            PdfDocumentRenderer pdfRenderer = new()
            {
                Document = document
            };
            pdfRenderer.RenderDocument();

            for (int idx = 0; idx < pdfRenderer.PdfDocument.Pages.Count; idx++)
            {
                var page = pdfRenderer.PdfDocument.Pages[idx];
                XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
                double margin = 20;
                double width = (double)page.Width.Value - 2 * margin;
                double height = (double)page.Height.Value - 2 * margin;
                gfx.DrawRectangle(new XPen(XColors.Black, 1), margin, margin, width, height);
            }

            pdfRenderer.PdfDocument.Save(path);

        }
        /// <summary>
        /// FrameworkElement(차트 등)를 이미지 파일(PNG)로 저장
        /// </summary>
        private (bool, string) SaveChartImage(FrameworkElement chart, string path)
        {
            int width = (int)chart.ActualWidth;
            int height = (int)chart.ActualHeight;
            if (width > 0 && height > 0)
            {
                RenderTargetBitmap bitmap = new RenderTargetBitmap(
                    width,
                    height,
                    96, 96,
                    PixelFormats.Pbgra32);
                bitmap.Render(chart);

                PngBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                using (FileStream fs = new(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }
                return (true, path);
            }
            return (false, string.Empty);
        }

        /// <summary>
        /// #RRGGBB hex 색상 문자열을 MigraDoc Color로 변환
        /// </summary>
        public MigraDoc.DocumentObjectModel.Color HexToColor(string hexColor)
        {
            var color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(hexColor);
            return MigraDoc.DocumentObjectModel.Color.FromRgb(color.R, color.G, color.B);
        }
    }

    /// <summary>
    /// 한글(맑은 고딕 등) 및 영문 폰트 파일을 로드해주는 폰트 
    /// </summary>
    public class FontResolver : IFontResolver
    {
        /// <summary>
        /// 폰트명(faceName)별로 폰트 파일 바이트 배열 반환
        /// </summary>
        public byte[] GetFont(string faceName)
        {
            if (faceName == "맑은 고딕#")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "malgun.ttf");
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "맑은 고딕#b")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "malgunbd.ttf");
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "맑은 고딕#i")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "malgunitalic.ttf");
                if (!File.Exists(fontPath))
                {
                    fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "malgun.ttf");
                }
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "맑은 고딕#bi")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "malgunbd.ttf");
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "Courier New#")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "cour.ttf");
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "Courier New#b")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "courbd.ttf");
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "Courier New#i")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "couri.ttf");
                return File.ReadAllBytes(fontPath);
            }
            else if (faceName == "Courier New#bi")
            {
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "courbi.ttf");
                return File.ReadAllBytes(fontPath);
            }

            return null;
        }

        /// <summary>
        /// 폰트명 매핑 반환
        /// </summary>
        public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic)
        {
            if (string.Equals(familyName, "맑은 고딕", StringComparison.OrdinalIgnoreCase))
            {
                string faceName = "맑은 고딕#";
                if (bold && italic)
                {
                    faceName = "맑은 고딕#bi";
                }
                else if (bold)
                {
                    faceName = "맑은 고딕#b";
                }
                else if (italic)
                {
                    faceName = "맑은 고딕#i";
                }

                return new FontResolverInfo(faceName);
            }
            else if (string.Equals(familyName, "Courier New", StringComparison.OrdinalIgnoreCase))
            {
                string faceName = "Courier New#";
                if (bold && italic)
                {
                    faceName = "Courier New#bi";
                }
                else if (bold)
                {
                    faceName = "Courier New#b";
                }
                else if (italic)
                { 
                    faceName = "Courier New#i";
                }
                return new FontResolverInfo(faceName);
            }
            return PlatformFontResolver.ResolveTypeface(familyName, bold, italic);
        }
    }

}