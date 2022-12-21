using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BookList
{
    public enum EnmCol
    {
        colNo = 0,
        colTitle,
        colPrice,
        colAuthor,
        colEnd
    }

    /// <summary>
    /// Form1 Class
    /// </summary>
    public partial class Form1 : Form
    {
        // クラスのメンバアイテム
        private List<BOOKDATA> ListBooks = null;
        private const string BOOK_DATA_FILE = @"C:\Temp\BookData.csv";
        private const int BOOK_ITEM_NUM = 4;  


        /// <summary>
        /// Form1 コンストラクタ
        /// インスタンスが生成された瞬間に実行されるメソッド
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// LOAD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // File Read
            Read_BooksCsv(BOOK_DATA_FILE, out ListBooks);

            // データをDataGridViewに設定
            dgvBook.Rows.Clear();       // DataGridViewアイテムクリア
            for (int i = 0; i < ListBooks.Count; i++)
            {
                // 行を追加
                int r = dgvBook.Rows.Add();
                dgvBook[(int)EnmCol.colNo, r].Value = ListBooks[i].iNo.ToString();
                dgvBook[(int)EnmCol.colTitle, r].Value = ListBooks[i].stTitle;
                dgvBook[(int)EnmCol.colPrice, r].Value = ListBooks[i].iPrice.ToString();
                dgvBook[(int)EnmCol.colAuthor, r].Value = ListBooks[i].stAuthor;
            }
        }

        
        /// <summary>
        /// CancelButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        /// <summary>
        /// OK Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            // ListBooks クリア
            ListBooks = new List<BOOKDATA>();

            // dgvBookの内容をList<BOOKDATA>に格納
            if (dgvBook.Rows.Count <= 1) return;    // 1:新規行

            for (int i = 0; i < dgvBook.RowCount - 1; i++)
            {
                BOOKDATA book = new BOOKDATA();
                if (dgvBook[(int)EnmCol.colNo, i].Value != null
                    && dgvBook[(int)EnmCol.colNo, i].Value.ToString().Length > 0)
                {
                    book.iNo = int.Parse(dgvBook[(int)EnmCol.colNo, i].Value.ToString());
                }
                if (dgvBook[(int)EnmCol.colTitle, i].Value != null
                    && dgvBook[(int)EnmCol.colTitle, i].Value.ToString().Length > 0)
                {
                    book.stTitle = dgvBook[(int)EnmCol.colTitle, i].Value.ToString();
                }
                if (dgvBook[(int)EnmCol.colPrice, i].Value != null
                    && dgvBook[(int)EnmCol.colPrice, i].Value.ToString().Length > 0)
                {
                    book.iPrice = int.Parse(dgvBook[(int)EnmCol.colPrice, i].Value.ToString());
                }
                if (dgvBook[(int)EnmCol.colAuthor, i].Value != null
                    && dgvBook[(int)EnmCol.colAuthor, i].Value.ToString().Length > 0)
                {
                    book.stAuthor = dgvBook[(int)EnmCol.colAuthor, i].Value.ToString();
                }

                ListBooks.Add(book);
            }

            if (ListBooks != null && ListBooks.Count > 0)
            {
                if (Write_BooksCsv(BOOK_DATA_FILE, ListBooks))
                {
                    MessageBox.Show("書込み完了しました。",
                                    "BookList",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("書込み失敗しました。",
                                    "BookList",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
        }

        
        /// <summary>
        /// CSVファイルを読込み、lstBooksにセットする
        /// </summary>
        /// <param name="flName"></param>
        /// <param name="lstBooks"></param>
        /// <returns></returns>
        public bool Read_BooksCsv(string flName, out List<BOOKDATA> lstBooks)
        {
            lstBooks = new List<BOOKDATA>();

            if (!File.Exists(flName)) return false;

            // File open
            using (StreamReader sr = new StreamReader(flName, Encoding.GetEncoding("shift_jis")))
            {
                string read_buf;

                // Read
                while ((read_buf = sr.ReadLine()) != null)
                {
                    string[] items = read_buf.Split(',');

                    if (items.Length < BOOK_ITEM_NUM) continue;

                    BOOKDATA book = new BOOKDATA();
                    if (items[0] != null && items[0].Length > 0)
                    {
                        book.iNo = int.Parse(items[0]);
                    }
                    if (items[1] != null && items[1].Length > 0)
                    {
                        book.stTitle = items[1];
                    }
                    if (items[2] != null && items[2].Length > 0)
                    {
                        book.iPrice = int.Parse(items[2]);
                    }
                    if (items[3] != null && items[3].Length > 0)
                    {
                        book.stAuthor = items[3];
                    }

                    lstBooks.Add(book);
                }

                // File close
                sr.Close();
            }

            return true;
        }


        /// <summary>
        /// lstBooksに格納されているデータをCSVファイルに書込む
        /// </summary>
        /// <param name="flName"></param>
        /// <param name="lstBooks"></param>
        /// <returns></returns>
        public bool Write_BooksCsv(string flName, List<BOOKDATA> lstBooks)
        {
            if (lstBooks == null || lstBooks.Count == 0) return false;

            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(flName, false, Encoding.GetEncoding("shift_jis"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            foreach (BOOKDATA book in lstBooks)
            {
                string rec_buff = book.iNo.ToString()
                                + ","
                                + book.stTitle
                                + ","
                                + book.iPrice.ToString()
                                + ","
                                + book.stAuthor;
                sw.WriteLine(rec_buff);
            }
            sw.Close();

            return true;
        }
    }

    
    /// <summary>
    /// BOOKDATA クラス
    /// </summary>
    public class BOOKDATA
    {
        public int iNo;
        public string stTitle;
        public int iPrice;
        public string stAuthor;
    }
}