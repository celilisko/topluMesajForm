using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Arayuz
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public string adiTanım;
        public string sifreniGir;

        private void Form2_Load(object sender, EventArgs e)
        {
            HosgendinTxt.Text = "Hoşgeldiniz "+adiTanım+ "...";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txt_mesaj.Text) && !String.IsNullOrEmpty(txt_numara.Text))
            {
                string url = "https://smsgw.mutlucell.com/smsgw-ws/sndblkex";
                var soapEnvelopeTemplate = CreateSoapEnvelope(OrginatorTxt.Text, txt_mesaj.Text, txt_numara.Text);
                var webRequest = CreateWebRequest(url, url);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        lbl_response.Text = soapResult;
                    }
                    if (lbl_response.Text == "20")
                    {
                        MessageBox.Show("Post Edilen Xml Eksik veya Hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (lbl_response.Text == "21")
                    {
                        MessageBox.Show("Kullanılan Originatöre Sahip Değilsiniz. ", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (lbl_response.Text == "22")
                    {
                        MessageBox.Show("Kontörünüz Yetersiz.)", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (lbl_response.Text == "23")
                    {
                        MessageBox.Show("Kullanıcı Adı veya Parolanız Hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (lbl_response.Text == "24")
                    {
                        MessageBox.Show("Şu anda size ait başka bir işlem aktif.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (lbl_response.Text == "25")
                    {
                        MessageBox.Show("SMSC Stopped (Bu hatayı alırsanız, işlemi 1-2 dk sonra tekrar deneyin.)", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (lbl_response.Text == "30")
                    {
                        MessageBox.Show("Hesap Aktivasyonu Sağlanmamış.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Mesajınız Başarıyla Gönderilmiştir. ", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private XmlDocument CreateSoapEnvelope(string OrginatorTxt, string mesajgirisi, string numbers)
        {

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml("<smspack ka='"+adiTanım+"' pwd='"+sifreniGir+"' org='" + OrginatorTxt + "'> <mesaj> <metin> " + mesajgirisi + " </metin> <nums>" + numbers + "</nums></mesaj>" + "</smspack>");
            return soapEnvelopeDocument;
        }

        private void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

       





        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(gsmTxt.Text) && !String.IsNullOrEmpty(aboneNotxt.Text))
            {
                string url = "https://smsgw.mutlucell.com/smsgw-ws/gtincmngapi";
                var soapEnvelopeTemplate = CreateSoapEnvelopeGelen(aboneNotxt.Text, gsmTxt.Text, startDateTxt.Text, endDateTxt.Text);
                var webRequest = CreateWebRequest(url, url);
                InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
                IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                string soapResult;
                using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                {
                    using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    {   
                        soapResult = rd.ReadToEnd();
                        if (!String.IsNullOrEmpty(soapResult))
                        {
                           
                            var messages = soapResult.Split(new string[] { "\n" }, StringSplitOptions.None).ToList();
                            if(messages!=null && messages.Count > 0)
                            {
                                messages.Where(x=> !String.IsNullOrEmpty(x)).ToList().ForEach(x => listBox1.Items.Add($"Tarih : { x.Split(new string[] { "\t" }, StringSplitOptions.None)[0]} - Numara : { x.Split(new string[] { "\t" }, StringSplitOptions.None)[1]} - Mesaj : { x.Split(new string[] { "\t" }, StringSplitOptions.None)[2]}"));
                            }
                        }
                        
                    }
                    if (listBox1.Text == "20")
                    {
                        MessageBox.Show("XML HATALI");
                    }
                    else if (listBox1.Text == "23")
                    {
                        MessageBox.Show("Abone Numarası yanlış.");
                    }
                    

                }
            }
        }
        private XmlDocument CreateSoapEnvelopeGelen(string myaboneno, string gsmNumara, string startDateTxt, string endDateTxt)
        {
            XmlDocument soapEnvelopeDocumentGelen = new XmlDocument();
            soapEnvelopeDocumentGelen.LoadXml("<increport aboneno='" + myaboneno + "' pwd ='" + sifreniGir + "' gsmno='" + gsmNumara + "' startdate='" + startDateTxt + "' enddate='" + endDateTxt + "' />");
            return soapEnvelopeDocumentGelen;
        }                                           /*("<smsorig ka='" + adiTanım + "' pwd='" + sifreniGir + "' />");*/

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lütfen hangi numaradan gelen smsleri görmek istiyorsanız o numarayı yazınız.", "Bilgilendirme Metni.", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lütfen abone numaranızı giriniz. Örn: 90850 5505555", "Bilgilendirme Metni.", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("*Öğrenmek istediğiniz tarihleri belirleyiniz.  \n *Yazım sıralaması YIL-AY-GÜN SAAT:DAKİKA Şeklindedir.", "Bilgilendirme Metni.", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string url = "https://smsgw.mutlucell.com/smsgw-ws/gtorgex";
            var soapEnvelopeTemplate = CreateSoapEnvelopeOrSorgula();
            var webRequest = CreateWebRequest(url, url);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    GelenTextOr.Text = soapResult;
                    OrginatorTxt.Clear();
                    OrginatorTxt.Text= GelenTextOr.Text.Trim();
                }
            }
        }
        private XmlDocument CreateSoapEnvelopeOrSorgula()
        {
            XmlDocument soapEnvelopeDocumentOrSorgula = new XmlDocument();
            soapEnvelopeDocumentOrSorgula.LoadXml("<smsorig ka='" + adiTanım + "' pwd='" + sifreniGir + "' />");
            return soapEnvelopeDocumentOrSorgula;
        }

        private void KontorSorButton_Click(object sender, EventArgs e)
        {
            string url = "https://smsgw.mutlucell.com/smsgw-ws/gtcrdtex";
            var soapEnvelopeTemplate = CreateSoapEnvelopeKontorSorgula();
            var webRequest = CreateWebRequest(url, url);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    kontorSorGelen.Text = soapResult;
                    if (kontorSorGelen.Text == "0")
                    {
                        MessageBox.Show("Lütfen web sitemizi ziyaret ediniz veya müşteri hizmetlerini arayınız. \n Arayüzün sağ üst köşesindeki Kontör Yükle Butonuna tıklıyarak web sitemize gidebiliriniz.", "Kontörünüz kalmamıştır.", MessageBoxButtons.OKCancel, MessageBoxIcon.Hand);
                    }

                    else
                    {
                        MessageBox.Show("Toplam  " + kontorSorGelen.Text + "  adet mesaj hakkınız kalmıştır.", "Kalan Kontörünüz", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                }
            }
        }
        private XmlDocument CreateSoapEnvelopeKontorSorgula()
        {
            XmlDocument soapEnvelopeDocumentKontorSorgula = new XmlDocument();
            soapEnvelopeDocumentKontorSorgula.LoadXml("<smskredi ka='" + adiTanım + "' pwd='" + sifreniGir + "' />");
            return soapEnvelopeDocumentKontorSorgula;
        }

        private void KaraSorgula_Click(object sender, EventArgs e)
        {
            SorguBox1.Items.Clear();
            string url = "https://smsgw.mutlucell.com/smsgw-ws/gtblklst";
            var soapEnvelopeTemplate = CreateSoapEnvelopeKaralisteSorgulama();
            var webRequest = CreateWebRequest(url, url);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    string adc = soapResult;
                    if (0 < adc.Length)
                    {
                        var numbers = adc.Split(new string[] { "\n" },StringSplitOptions.None);
                        if(numbers !=null && numbers.Length > 0)
                        {
                            foreach (var number in numbers)
                            {
                                if (!String.IsNullOrEmpty(number))
                                {
                                    SorguBox1.Items.Add(number);
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Kara listenize ekli numara bulunmamaktadır.", "Bilgilendirme Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }

                }
                
            }
        }
        private XmlDocument CreateSoapEnvelopeKaralisteSorgulama()
        {
            XmlDocument soapEnvelopeDocumentKaralisteSorgulama = new XmlDocument();
            soapEnvelopeDocumentKaralisteSorgulama.LoadXml("<blacklist ka='" + adiTanım + "' pwd='" + sifreniGir + "' />");
            return soapEnvelopeDocumentKaralisteSorgulama;
        }

        private void KaraEkle_Click(object sender, EventArgs e)
        {
            string url = "https://smsgw.mutlucell.com/smsgw-ws/addblklst";
            var soapEnvelopeTemplate = CreateSoapEnvelopeKaralisteEkleme(KaraListeTxt.Text);
            var webRequest = CreateWebRequest(url, url);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    kleklemeTxt.Text = soapResult;
                    if (kleklemeTxt.Text == "20")
                    {
                        MessageBox.Show("Post edilen xml yanlış veya hatalı.", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        MessageBox.Show("Numara/Numaralar kara listeye eklendi.", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }

                }

            }
        }
        private XmlDocument CreateSoapEnvelopeKaralisteEkleme(string karalistenumara)
        {
            XmlDocument soapEnvelopeDocumentKaralisteEkleme = new XmlDocument();
            soapEnvelopeDocumentKaralisteEkleme.LoadXml("<addblacklist ka ='"+adiTanım+"' pwd='"+sifreniGir+"'> <nums> "+karalistenumara+"</nums>"+"</addblacklist >");
            return soapEnvelopeDocumentKaralisteEkleme;
        }

        private void KaraCıkar_Click(object sender, EventArgs e)
        {
            string url = "https://smsgw.mutlucell.com/smsgw-ws/dltblklst";

            if (String.IsNullOrEmpty(KaraListeTxt.Text))
            {
                MessageBox.Show("Numara girişi yapmalısınız");
                return;
            }
               
            var soapEnvelopeTemplate = CreateSoapEnvelopeKaralisteCıkarma(KaraListeTxt.Text);
            var webRequest = CreateWebRequest(url, url);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeTemplate, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                    kleklemeTxt.Text = soapResult;
                    if (kleklemeTxt.Text == "20")
                    {
                        MessageBox.Show("Post edilen xml yanlış veya hatalı.", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }
                    else
                    {
                        MessageBox.Show("Numara/Numaralar kara listeden silindi.", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    }

                }

            }
        }
        private XmlDocument CreateSoapEnvelopeKaralisteCıkarma(string karalistenumarasil)
        {
            XmlDocument soapEnvelopeDocumentKaralisteCıkarma = new XmlDocument();
            soapEnvelopeDocumentKaralisteCıkarma.LoadXml("<dltblacklist ka ='" + adiTanım + "' pwd='" + sifreniGir + "'> <nums> " + karalistenumarasil + "</nums>" + "</dltblacklist >");
            return soapEnvelopeDocumentKaralisteCıkarma;
        }

        private void ComboHazır_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboHazır.SelectedIndex == 0)
            {
                HazırBox1.Clear();
                HazırBox1.Text = "Modern Türkiyenin Genc Mimarlari, Genclik Bayraminiz Kutlu Olsun.";
            }
            else if (ComboHazır.SelectedIndex == 1)
            {
                HazırBox1.Clear();
                HazırBox1.Text = "Yüce ulusumuzun 30 agustos zafer bayramini kutlar, cumhuriyetimizin ilelebet varligini temenni ederiz.";
            }
            else if (ComboHazır.SelectedIndex == 2)
            {
                HazırBox1.Clear();
                HazırBox1.Text = "Yasaminizda güzel yillar, mutlu yarinlar sizinle olsun. MUTLU YILLAR!";
            }
            else if (ComboHazır.SelectedIndex == 3)
            {
                HazırBox1.Clear();
                HazırBox1.Text = "Kandilinizi kutlar hayirlara vesile olmasi dilegiyle sevgi ve saygilarimi sunarim.";
            }
            else if (ComboHazır.SelectedIndex == 4)
            {
                HazırBox1.Clear();
                HazırBox1.Text = "Ramazaninizin bereket, mutluluk, saglik ve afiyet getirmesini dileriz. Nice Ramazanlara";
            }
            else if (ComboHazır.SelectedIndex == 5)
            {
                HazırBox1.Clear();
                HazırBox1.Text = "Yeni yilin tüm insanliga ve ülkemize baris, mutluluk getirmesi dilegiyle yeni yiliniz kutlu olsun.";
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            txt_mesaj.Clear();
            txt_mesaj.Text = HazırBox1.Text;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Numara ekle ve çıkarma yaparken 10 haneli olacak şekilde başında sıfır olmadan yazınız.\nBirden fazla numara yollamak için 2 numara arasına virgül koyunuz. \nörn:5003861161,5005487652", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Numarayı ve numaraları 10 haneli olacak şekilde başında sıfır olmadan yazınız.\nBirden fazla numaraya mesaj yollamak için 2 numara arasına virgül koyunuz. \nörn:5003861161,5005487652", "Bilgilendirme Metni", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
    }
}