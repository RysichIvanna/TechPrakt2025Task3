using iTextSharp.text.pdf;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Xml.Linq;

namespace lab
{
    public partial class MainWindow : RibbonWindow
    {
        public bool IsSaved = false;
        public bool IsPrinted = false;

        private ColorHandler colorHandler = new ColorHandler(System.Windows.Media.Brushes.Blue);

        private XpsDocument xpsDoc;

        public Color SelectedColor { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            _fontSize.ItemsSource = TextChangeHandler.FontSizes;

            _fontSize.SelectedItem = TextChangeHandler.FontSizes.First();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Document files (*.doc)|*.doc";
            var result = dlg.ShowDialog();
            TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            if (result.Value)
            {
                SaveLoad.LoadDocumentFromFile(dlg.FileName, textRange);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();

            string currentDateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            savefile.FileName = currentDateTime;

            TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);

            savefile.Filter = "Document files (*.doc)|*.doc";
            if (savefile.ShowDialog() == true)
            {
                var result = SaveLoad.SaveDocumentToFile(savefile.FileName, textRange);
                this.Title = $"{Utilily.GetFileNameFromPath(result.fileName)}";
                IsSaved = result.isSaved;
            }
        }



        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    TextRange range = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
                    range.Save(stream, DataFormats.XamlPackage);

                    using (Package package = Package.Open(stream, FileMode.Create))
                    {
                        using (XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed))
                        {
                            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                            DocumentPaginator paginator = ((IDocumentPaginatorSource)xpsDoc.GetFixedDocumentSequence()).DocumentPaginator;
                            xpsWriter.Write(paginator);
                        }
                    }

                    printDialog.PrintDocument(((IDocumentPaginatorSource)xpsDoc.GetFixedDocumentSequence()).DocumentPaginator, "Document");
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            AskToSaveChanges();

            _richTextBox.Document.Blocks.Clear();
        }

        private bool AskToSaveChanges()
        {
            if (!IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Message", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    btnSave_Click(this, new RoutedEventArgs());
                }
                return true;
            }
            return false;
        }

        private void AppClosing(object sender, RoutedEventArgs e)
        {
            if (AskToSaveChanges())
            {
                this.Close();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!AskToSaveChanges())
            {
                e.Cancel = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            AppClosing(sender, e);
        }

        void ApplyPropertyValueToSelectedText(DependencyProperty formattingProperty, object
value)
        {
            if (value == null)
                return;
            _richTextBox.Selection.ApplyPropertyValue(formattingProperty, value);
        }
        private void FontFamili_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FontFamily editValue = (FontFamily)e.AddedItems[0];
                ApplyPropertyValueToSelectedText(TextElement.FontFamilyProperty, editValue);
            }
            catch (Exception) { }
        }
        private void FontSize_SelectionChange(object sender, SelectionChangedEventArgs e)
        {

            try
            {


                ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, e.AddedItems[0]);
            }
            catch (Exception) { }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog();
        }

        private void FontColorButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyPropertyValueToSelectedText(TextElement.ForegroundProperty, (SolidColorBrush)ColorHandler.ChangeTextColor());
        }

        private void InsertImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.gif)|*.png;*.jpg;*.gif|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;

                Image image = new Image();
                image.Source = new BitmapImage(new Uri(imagePath));
                image.Width = 100;

                BlockUIContainer container = new BlockUIContainer(image);

                _richTextBox.Document.Blocks.Add(container);
            }
        }
        private void InsertTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(rowCountTextBox.Text, out int rowCount) && int.TryParse(columnCountTextBox.Text, out int columnCount))
            {
                Table table = new Table();

                for (int i = 0; i < rowCount; i++)
                {
                    TableRow row = new TableRow();

                    for (int j = 0; j < columnCount; j++)
                    {
                        TableCell cell = new TableCell(new Paragraph(new Run("Cell " + (i + 1) + "-" + (j + 1))));
                        cell.BorderBrush = Brushes.Black;
                        cell.BorderThickness = new Thickness(1);
                        row.Cells.Add(cell);
                    }

                    TableRowGroup group = new TableRowGroup();
                    group.Rows.Add(row);
                    table.RowGroups.Add(group);
                }

                FlowDocument flowDocument = new FlowDocument();
                flowDocument.Blocks.Add(table);
                _richTextBox.Document = flowDocument;
            }
            else
            {
                MessageBox.Show("Invalid row or column count. Please enter valid numbers.");
            }
        }


        private void AddBulletList_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(listCountTextBox.Text, out int listC))
            {
                TextChangeHandler.AddList(listC, _richTextBox);
            }
            else
            {
                MessageBox.Show("Invalid list count. Please enter valid numbers.");
            }
        }

        private void OnGetFileSizeClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"File size: {Utilily.GetFileSize(_richTextBox)}");
        }

        private void AddRectangle_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(widthTextBox.Text, out int width) && int.TryParse(heightTextBox.Text, out int height))
            {
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                rectangle.Width = width;
                rectangle.Height = height;
                rectangle.Fill = colorHandler.GetShapeColor();

                BlockUIContainer container = new BlockUIContainer(rectangle);
                _richTextBox.Document.Blocks.Add(container);
            }
            else
            {
                MessageBox.Show("Invalid width or height. Please enter valid numbers.");
            }
        }
        private void AddCircle_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(widthTextBox.Text, out int width) && int.TryParse(heightTextBox.Text, out int height))
            {
                System.Windows.Shapes.Ellipse ellipse = new System.Windows.Shapes.Ellipse();
                ellipse.Width = width;
                ellipse.Height = height;
                ellipse.Fill = colorHandler.GetShapeColor();

                BlockUIContainer container = new BlockUIContainer(ellipse);
                _richTextBox.Document.Blocks.Add(container);
            }
            else
            {
                MessageBox.Show("Invalid width or height. Please enter valid numbers.");
            }
        }

        private void AddLine_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            line.Stroke = colorHandler.GetShapeColor();
            line.StrokeThickness = 2;

            line.X1 = 50;
            line.Y1 = 50;
            line.X2 = 150;
            line.Y2 = 150;

            BlockUIContainer container = new BlockUIContainer(line);
            _richTextBox.Document.Blocks.Add(container);
        }

        private void PickShapeColor_Click(object sender, RoutedEventArgs e)
        {
            colorHandler.SetShapeBrushColor();
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                Hyperlink hyperlink = new Hyperlink(new Run(filePath));
                hyperlink.NavigateUri = new Uri(filePath);
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;

                Paragraph paragraph = new Paragraph(hyperlink);
                _richTextBox.Document.Blocks.Add(paragraph);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void AddLineChart_Click(object sender, RoutedEventArgs e)
        {
            ChartValues<ObservableValue> values = new ChartValues<ObservableValue>
            {
                new ObservableValue(4),
                new ObservableValue(6),
                new ObservableValue(5),
                new ObservableValue(2),
                new ObservableValue(7)
            };

            CartesianChart chart = new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Series 1",
                        Values = values
                    }
                }
            };

            chart.AxisX.Add(new Axis { Title = "X Axis" });
            chart.AxisY.Add(new Axis { Title = "Y Axis", LabelFormatter = value => value.ToString() });


            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(100, 100, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(chart);


            Image image = new Image
            {
                Source = renderTargetBitmap,
                Stretch = Stretch.None
            };

            BlockUIContainer container = new BlockUIContainer(image);
            _richTextBox.Document.Blocks.Add(container);
        }

        private void InsertText(string text, TextPointer start = null, TextPointer end = null)
        {
            TextRange textRange = new TextRange(_richTextBox.Selection.Start, _richTextBox.Selection.End);
            if (start != null && end != null)
            {
                textRange = new TextRange(start, end);
            }
            textRange.Text = text;
        }

        private void InsertAlpha_Click(object sender, RoutedEventArgs e)
        {
            InsertText("α");
        }

        private void InsertBeta_Click(object sender, RoutedEventArgs e)
        {
            InsertText("β");
        }

        private void InsertMu_Click(object sender, RoutedEventArgs e)
        {
            InsertText("µ");
        }

        private void HighlightColorButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyPropertyValueToSelectedText(TextElement.BackgroundProperty, ColorHandler.ChangeHightlightColor());
        }
        private void FindText_Click(object sender, RoutedEventArgs e)
        {
            string searchText = searchTextbox.Text;

            TextPointer start = _richTextBox.Selection.IsEmpty ? _richTextBox.Document.ContentStart : _richTextBox.Selection.Start;
            TextPointer end = _richTextBox.Document.ContentEnd;

            var results = TextChangeHandler.FindText(searchText, start, end);
            if (results.endPos == null || results.endPos == null)
            {
                MessageBox.Show("Nothing");
            }
            else
            {
                _richTextBox.Selection.Select(results.startPos, results.endPos);
            }
        }


        private void ReplaceText_Click(object sender, RoutedEventArgs e)
        {
            string searchText = searchTextbox.Text;
            string replaceText = replaceTextbox.Text;

            bool isTrue = TextChangeHandler.ReplaceText(searchText, replaceText, _richTextBox);

            if (!isTrue)
            {
                MessageBox.Show("Text not found.");
            }
        }

        private void aSortBtn_Click(object sender, RoutedEventArgs e)
        {
            TextPointer start = _richTextBox.Selection.IsEmpty ? _richTextBox.Document.ContentStart : _richTextBox.Selection.Start;
            TextPointer end = _richTextBox.Document.ContentEnd;

            TextChangeHandler.SortText(start, end, _richTextBox);
        }
    }
}