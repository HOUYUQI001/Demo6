using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Kingdee.BOS.WebApi.Client;
namespace Demo6
{
    //
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string getJson;
        string gofServerAdd;
        string key;
        string url = "";
        //MES
        JArray ProductJAMes = new JArray();
        JArray MouldJAMes = new JArray();
        JArray MachineJAMes = new JArray();
        JArray PlanJAMes = new JArray();
        JArray TaskJAMes = new JArray();
        JArray MesMouldRes = new JArray();
        JArray MesProductRes = new JArray();
        JArray MesPlanRes = new JArray();
        JArray MesTaskRes = new JArray();
        //Kd
        JArray ProductJAKd = new JArray();
        JArray MouldJAKd = new JArray();
        JArray MachineJAKd = new JArray();
        JArray PlanJAKd = new JArray();
        JArray TaskJAKd = new JArray();
        Dictionary<string, string> parasMes = new Dictionary<string, string>();

        List<object> ParametersKd = new List<object>();
        private void button1_Click(object sender, EventArgs e)
        {
            JHDateTimePicker.Visible = true;
            /// <summary>
            /// 小诸葛登录
            /// </summary>
            url = "http://js1.gomake.cn/kq_highnet_gof_connect/gof1.0/authorize";
            parasMes.Add("appid", "ff20267f88954ed4b30f6846512c028f");
            parasMes.Add("ver", "1.0");
            parasMes.Add("seq", DateTime.Now.ToString());
            getJson = HttpUitls.Get(url, parasMes);
            //这个需要引入Newtonsoft.Json这个DLL并using45252
            //传入我们的实体类还有需要解析的JSON字符串这样就OK了。然后就可以通过实体类使用数据了。
            // rt = JsonConvert.DeserializeObject<MouldRootobject>(getJson);
            var addressDO = JsonConvert.DeserializeObject<dynamic>(getJson);

            //Rootobject rt = JsonConvert.DeserializeObject<Rootobject>(getJson);
            //这样就可以取出json数据里面的值
            //gofServerAdd = rt.data.gofServerAdd;
            gofServerAdd = addressDO.data.gofServerAdd;
            url = gofServerAdd + "/gof1.0/verify";
            parasMes.Clear();
            parasMes.Add("appid", "ff20267f88954ed4b30f6846512c028f");
            parasMes.Add("ver", "1.0");
            parasMes.Add("seq", DateTime.Now.ToString());
            getJson = HttpUitls.Get(url, parasMes);
            var verifyDO = JsonConvert.DeserializeObject<dynamic>(getJson);
            //rt = JsonConvert.DeserializeObject<MouldRootobject>(getJson);
            //MessageBox.Show(rt.data.verificationCode);
            //由于这个JSON字符串的 public List<DataItem> data 是一个集合，所以我们需要遍历集合里面的所有数据
            url = gofServerAdd + "/gof1.0/key";
            string Md5verificationCode = Md5.GetMd5(verifyDO.data.verificationCode.ToString());
            string Md5apipwd = Md5.GetMd5("ACD612DF");
            string Md5pwd = Md5.GetMd5(Md5verificationCode + Md5apipwd);
            parasMes.Clear();
            parasMes.Add("appid", "ff20267f88954ed4b30f6846512c028f");
            parasMes.Add("ver", "1.0");
            parasMes.Add("seq", DateTime.Now.ToString());
            parasMes.Add("pwd", Md5pwd);
            getJson = HttpUitls.Get(url, parasMes);
            //rt = JsonConvert.DeserializeObject<MouldRootobject>(getJson);
            var keyDO = JsonConvert.DeserializeObject<dynamic>(getJson);
            key = keyDO.data.key;
            /// <summary>
            /// 金蝶登录
            /// </summary>
            HttpClient httpClient = new HttpClient();
            httpClient.Url = "http://140.249.17.246:6888/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.AuthService.ValidateUser.common.kdsvc";
            List<object> ParametersKd = new List<object>();
            ParametersKd.Add("5c7406b02cae17");//帐套Id 测试：5e5c9dcfeb4892；正式：5c7406b02cae17
            ParametersKd.Add("侯宇琦");//用户名
            ParametersKd.Add("039216");//密码
            ParametersKd.Add(2052);
            httpClient.Content = JsonConvert.SerializeObject(ParametersKd);
            var iResult = JObject.Parse(httpClient.AsyncRequest())["LoginResultType"].Value<int>();
            if (iResult == 1 && keyDO.msg.ToString() == "ok")
            {
                MessageBox.Show("登录成功");//todo:验证成功，处理业务

            }
            else if (iResult != 1)
            {

                MessageBox.Show("金蝶登录不成功" + iResult.ToString());//todo:验证成功，处理业务
            }
            else if (keyDO.msg.ToString() == "ok")
            {

                MessageBox.Show("MES登录不成功" + keyDO.ToString());//todo:验证成功，处理业务
            }

            /// <summary>
            /// 小诸葛取模具信息
            /// </summary>
            //拼地址串
            url = "";
            url = gofServerAdd + "/v1/GET/getMoldAll";

            //创建一个字典类型数据来，传递参数
            parasMes.Clear();
            parasMes.Add("key", key);
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, parasMes);
            JObject MouldJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
            MouldJAMes = MouldJOMes["data"]["mould"] as JArray;

            /// <summary>
            /// 金蝶取模具信息
            /// </summary>
            //HttpClient httpClient = new HttpClient();
            //httpClient.clear;
            //服务地址
            httpClient.Url = "http://140.249.17.246:6888/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExecuteBillQuery.common.kdsvc";
            //Json字串
            //把查询语句写入
            string data = "{\"FormId\":\"BD_MATERIAL\",\"FilterString\":\"FNumber like '6%' and FUSEORGID = '100019'and FFORBIDSTATUS = 'A' AND FDocumentStatus = 'C'\",\"FieldKeys\":\"FNAME,F_WW_MJMJDM,F_WW_MJMJZQS\"}";
            ParametersKd.Clear();
            ParametersKd.Add(data);
            //序列化上面数据返回结果
            httpClient.Content = JsonConvert.SerializeObject(ParametersKd);
            var result = httpClient.AsyncRequest();
            MouldJAKd = JArray.Parse(result);


            /// <summary>
            /// 小诸葛取产品信息
            /// </summary>
            //拼地址串
            url = "";
            url = gofServerAdd + "/v1/GET/getProductAll";
            //创建一个字典类型数据来，传递参数
            parasMes.Clear();
            parasMes.Add("key", key);
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, parasMes);
            JObject ProductJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
            ProductJAMes = ProductJOMes["data"]["product"] as JArray;

            /// <summary>
            /// 金蝶取产品信息
            /// </summary>
            //服务地址
            httpClient.Url = "http://140.249.17.246:6888/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExecuteBillQuery.common.kdsvc";
            //Json字串
            //把查询语句写入
            data = "";
            data = "{\"FormId\":\"BD_MATERIAL\",\"Limit\":0,\"FilterString\":\" (FNumber like '2%' or FNumber like '3%' )  and FUSEORGID = '100019' and FFORBIDSTATUS = 'A' AND FDocumentStatus = 'C'\",\"FieldKeys\":\"FNumber,FNAME,F_WW_ZSSJZQ,FDocumentStatus\"}";
            ParametersKd.Clear();
            ParametersKd.Add(data);
            //序列化上面数据返回结果
            httpClient.Content = JsonConvert.SerializeObject(ParametersKd);
            result = httpClient.AsyncRequest();
            ProductJAKd = JArray.Parse(result);

            /// <summary>
            /// 小诸葛取机台号
            /// </summary>
            url = gofServerAdd + "/v1/GET/getMachineAll";
            //创建一个字典类型数据来，传递参数
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras.Clear();
            paras.Add("key", key);
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, paras);
            JObject MachineJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
            MachineJAMes = MachineJOMes["data"]["machine"] as JArray;

            /// <summary>
            /// 小诸葛取计划单列表
            /// </summary>
            //拼地址串
            url = gofServerAdd + "v1/GET/getPlanOrder";
            //创建一个字典类型数据来，传递参数

            parasMes.Clear();
            parasMes.Add("key", key);
            
            parasMes.Add("date", JHDateTimePicker.Text+ " 00:00:00");
            parasMes.Add("indexPage", "1");
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, parasMes);
            JObject PlanJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
            

            if (PlanJOMes["data"].Type != JTokenType.Null)
            {
                for (int i = 1; i <= int.Parse(PlanJOMes["data"][0]["pageCount"].ToString()); i++)
                {
                    parasMes.Clear();
                    parasMes.Add("key", key);
                    parasMes.Add("date", JHDateTimePicker.Text + " 00:00:00");
                    parasMes.Add("indexPage", i.ToString());
                    //传入HttpUitls类，并取得返回值
                    getJson = HttpUitls.Get(url, parasMes);
                    PlanJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
                    foreach (var item in PlanJOMes["data"][0]["plan"])
                    {
                        PlanJAMes.Add(item);
                    }

                }
            }
            getJson = PlanJAMes.ToString();
            /// <summary>
            /// 金蝶取计划单号
            /// </summary>
            httpClient = new HttpClient();
            //服务地址   fplanstartdate >= ( convert(varchar(10),GETDATE(),120)+' 00:00:00') and fplanstartdate <= (convert(varchar(10), GETDATE(), 120) + ' 23:59:00')
            httpClient.Url = "http://140.249.17.246:6888/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExecuteBillQuery.common.kdsvc";
            //Json字串
            //把查询语句写入

            data = "{\"FormId\":\"PRD_MO\",\"FilterString\":\" fplanstartdate >= '"+JHDateTimePicker.Text + " 00:00:00' and  fplanstartdate  <= '" + JHDateTimePicker.Text + " 23:59:59' AND FDocumentStatus = 'C' \",\"FieldKeys\":\"FBILLNO,FTreeEntity_FSEQ,FDate,FSaleOrderEntrySeq,FMATERIALID.Fnumber,FMaterialName,F_HBL_ZSMJDM_T, FQTY,FPLANSTARTDATE,FPLANFINISHDATE,F_HBL_MJZQS,F_WW_BANCI.Fname,F_WW_JITAI.Fname\"}";
            ParametersKd.Clear();
            ParametersKd.Add(data);
            //序列化上面数据返回结果
            httpClient.Content = JsonConvert.SerializeObject(ParametersKd);
            result = httpClient.AsyncRequest();
            PlanJAKd = JArray.Parse(result);

            

            ///// < summary >
            ///// 金蝶取任务单号:取生产计划单数据即可，在计划单已经取过
            ///// </ summary >
            //JArray TaskJAkd = new JArray();
            //TaskJAKd = PlanJAKd;
            MessageBox.Show("获取数据成功");
            JHDateTimePicker.Visible = false;

        }

        private void button2_Click(object sender, EventArgs e)
        
        {

            //判断MES中有没有kd 的各个模具码，若没有加到mes中
            /// <summary>
            /// 总结mes中不存在的KD模具
            /// </summary>
            JArray MouldJAAdd = new JArray();
            for (int i = 0; i < MouldJAKd.Count; i++)
            {
                //int a = -1;
                int a = MouldJAMes.TakeWhile(x => x["moldNo"].ToString() != MouldJAKd[i][1].ToString()).Count();
                if (a >= MouldJAMes.Count())
                {
                    MouldJAAdd.Add(MouldJAKd[i]);
                }
            }
            //MessageBox.Show(MouldJAAdd.ToString()); 
            url = gofServerAdd + "/v1/POST/addMold";
            for (int i = 0; i < MouldJAAdd.Count; i++)
            {
                parasMes = new Dictionary<string, string>();
                parasMes.Add("key", key);
                parasMes.Add("moldName", MouldJAAdd[i][0].ToString());
                parasMes.Add("moldNo", MouldJAAdd[i][1].ToString());
                parasMes.Add("output", MouldJAAdd[i][2].ToString());
                //Rootobject rt = JsonConvert.DeserializeObject<Rootobject>(getJson);
                string postJson = HttpUitls.Post(url, parasMes);
               // MessageBox.Show(postJson);//todo:验证成功，处理业务
                JObject MoldPostRes = (JObject)JsonConvert.DeserializeObject(postJson);
                MoldPostRes.Add("moldName", MouldJAAdd[i][0].ToString());
                MoldPostRes.Add("moldNo", MouldJAAdd[i][1].ToString());
                MoldPostRes.Add("output", MouldJAAdd[i][2].ToString());
                MesMouldRes.Add(MoldPostRes);
                //MesMouldRes.Add()
                //if (MoldPostRes["code"] == 1)
                //{

                //}
            }
            int b = 0;
            int c = 0;
            string UploadSuccess = "";
            string UploadFailure = "";
            foreach (var Item in MesMouldRes)
            {
                
                
                if (Item["code"].ToString() == "1")
                {
                    b++;
                    UploadSuccess = UploadSuccess + "模具名称[" + Item["moldName"].ToString() + "]模具号为[" + Item["moldNo"].ToString() + "]单模产量为[" + Item["output"].ToString() + "]上传成功";

                }
                else
                {
                    c++;
                    UploadFailure = UploadFailure + "模具名称[" + Item["moldName"].ToString() + "]模具号为[" + Item["moldNo"].ToString() + "]单模产量为[" + Item["output"].ToString() + "]上传失败" + "原因为：" + Item["msg"].ToString() + "\r\n";
                }
                
            }
            MessageBox.Show("共上传成功" + b.ToString() + "条\r\n" + UploadSuccess + "共上传失败" + c.ToString() + "条\r\n" + UploadFailure);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //判断MES中有没有kd 的各个产品码，若没有加到mes中
            /// <summary>
            /// 总结mes中不存在的KD模具
            /// </summary>
            JArray ProductJAAdd = new JArray();
            for (int i = 0; i < ProductJAKd.Count; i++)
            {
                //int a = -1;
                int a = ProductJAMes.TakeWhile(x => x["partnum"].ToString() != ProductJAKd[i][0].ToString()).Count();
                if (a >= ProductJAMes.Count())
                {
                    ProductJAAdd.Add(ProductJAKd[i]);
                }
            }
            //MessageBox.Show(ProductJAAdd.ToString());
            url = gofServerAdd + "/v1/POST/AddProduct";
            for (int i = 0; i < ProductJAAdd.Count; i++)
            {
                parasMes = new Dictionary<string, string>();
                parasMes.Add("key", key);
                parasMes.Add("productName", ProductJAAdd[i][1].ToString());
                parasMes.Add("partNum", ProductJAAdd[i][0].ToString());
                parasMes.Add("cycletime", ProductJAAdd[i][2].ToString());
                //Rootobject rt = JsonConvert.DeserializeObject<Rootobject>(getJson);
                string postJson = HttpUitls.Post(url, parasMes);
                JObject ProductPostRes = (JObject)JsonConvert.DeserializeObject(postJson);
                ProductPostRes.Add("productName", ProductJAAdd[i][1].ToString());
                ProductPostRes.Add("partNum", ProductJAAdd[i][0].ToString());
                ProductPostRes.Add("cycletime", ProductJAAdd[i][2].ToString());
                MesProductRes.Add(ProductPostRes);
            }
            int b = 0;
            int c = 0;
            string UploadSuccess = "";
            string UploadFailure = "";
            foreach (var Item in MesMouldRes)
            {


                if (Item["code"].ToString() == "1")
                {
                    b++;
                    UploadSuccess = UploadSuccess + "产品名称[" + Item["productName"].ToString() + "]物料号为[" + Item["partNum"].ToString() + "]实际注塑时间为[" + Item["cycletime"].ToString() + "]上传至MES成功";

                }
                else
                {
                    c++;
                    UploadFailure = UploadFailure + "产品名称[" + Item["productName"].ToString() + "]物料号为[" + Item["partNum"].ToString() + "]实际注塑时间为[" + Item["cycletime"].ToString() + "]上传至MES失败" + "原因为：" + Item["msg"].ToString() + "\r\n";
                }

            }
            MessageBox.Show("共上传成功" + b.ToString() + "条\r\n" + UploadSuccess + "共上传失败" + c.ToString() + "条\r\n" + UploadFailure);



        }

        private void button4_Click(object sender, EventArgs e)
        {

            //判断MES中有没有kd中的各个产品码，若没有加到mes中
            /// < summary >
            /// 总结mes中不存在的KD模具
            /// </ summary >
            string NotUpload = "";
            JArray PlanJAAdd = new JArray();
            int d = 0;
            //bool aaa = PlanJAMes[2]["planNumber"].ToString() == (PlanJAKd[0][0].ToString() + PlanJAKd[0][1].ToString());
            
            for (int i = 0; i < PlanJAKd.Count; i++)
            {
                int a = -1;
                a = PlanJAMes.TakeWhile(x => x["planNumber"].ToString() != (PlanJAKd[i][0].ToString()+ PlanJAKd[i][1].ToString())).Count();//判断MES中有无金蝶生产订单号，若无，拼接mouldId,ProductId,MachineId
                if (a >= PlanJAMes.Count())//在PlanJAMes中查找不到的话a的值会大于等于PlanJAMes的count,需要在MES中新增计划单
                {

                    /// <summary>
                    /// 取MESlist中的moldId productId
                    /// </summary>
                    JArray PlanJANotAdd = new JArray();
                    int b = -1;
                    int c = -1;
                    int f = -1;

                    b = MouldJAMes.TakeWhile(x => x["moldNo"].ToString() != PlanJAKd[i][6].ToString()).Count();//判断第i个PlanJAKd的moldNo在MESMouldlist中第几个
                    
                    c = ProductJAMes.TakeWhile(x => x["partnum"].ToString() != PlanJAKd[i][4].ToString()).Count();//判断第i个PlanJAKd的moldNo在MESproductlist中第几个

                    f = MachineJAMes.TakeWhile(x => x["machineSN"].ToString() != PlanJAKd[i][12].ToString()).Count();//判断第i个TaskJAKd的moldNo在MESTasklist中第几个
                    if (b < MouldJAMes.Count() && c < ProductJAMes.Count() && f < MachineJAMes.Count())
                    {
                        PlanJAAdd.Add(PlanJAKd[i]);
                        //根据b(在meslist中的编号)把moldid加到PlanJAAdd中
                        PlanJAAdd[d][0].AddAfterSelf(MouldJAMes[b]["moldId"]);

                        //根据b(在meslist中的编号)把productid加到PlanJAAdd中
                        PlanJAAdd[d][0].AddAfterSelf(ProductJAMes[c]["productId"]);

                        PlanJAAdd[d][3].Replace(PlanJAAdd[d][0].ToString() + PlanJAAdd[d][3].ToString());//根据FTreeEntity_FSEQ的位置确定位置
                        d++;
                    }
                    else if (b >= MouldJAMes.Count()) 
                    {
                        //解释说明未同步mould信息，需要先同步mould信息
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，模具号为" + PlanJAKd[i][6].ToString() + "的模具在MES的模具列表中不存在！\r\n";
                    }
                    else if (c >= ProductJAMes.Count())
                    {
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，物料号为" + PlanJAKd[i][4].ToString() + "的物料在MES的物料列表中不存在！\r\n";
                        //解释说明未同步product信息，需要先同步product信息
                    }
                    else if (f >= MachineJAMes.Count())
                    {
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，机台为" + PlanJAKd[i][12].ToString() + "的机台在MES的机台列表中不存在！\r\n"; ;

                        //PlanJANotAdd[i].AddAfterSelf("Machine");
                        //解释说明未同步product信息，需要先同步product信息
                        //MessageBox.Show(MachineJAMes[i][0].ToString() + "中的第" + TaskJAKd[i][1].ToString() + "列，物料号为" + TaskJAKd[i][12].ToString() + "的物料在MES的机台列表中不存在！");
                    }
                }

            }
            //Dictionary<string, string> paras = new Dictionary<string, string>();
            MessageBox.Show(NotUpload);
            url = gofServerAdd + "v1/POST/addPlan";
            for (int i = 0; i < PlanJAAdd.Count; i++)
            {
                parasMes = new Dictionary<string, string>();
                parasMes.Add("key", key);
                parasMes.Add("planNumber", PlanJAAdd[i][3].ToString());
                parasMes.Add("productId", PlanJAAdd[i][1].ToString());
                parasMes.Add("moldId", PlanJAAdd[i][2].ToString());
                parasMes.Add("planQty", PlanJAAdd[i][9].ToString());
                parasMes.Add("orderNumber", PlanJAAdd[i][0].ToString());
                parasMes.Add("planStartDate", PlanJAAdd[i][10].ToString().Replace('/','-'));
                parasMes.Add("planEndDate", PlanJAAdd[i][11].ToString().Replace('/', '-'));
                string postJson = HttpUitls.Post(url, parasMes);
                //MessageBox.Show(parasMes.ToString() + postJson);//todo:验证成功，处理业务
                JObject PlanPostRes = (JObject)JsonConvert.DeserializeObject(postJson);
                //MesPlanRes
                PlanPostRes.Add("planNumber", PlanJAAdd[i][3].ToString());
                PlanPostRes.Add("planQty", PlanJAAdd[i][9].ToString());
                PlanPostRes.Add("orderNumber", PlanJAAdd[i][0].ToString());
                PlanPostRes.Add("planStartDate", PlanJAAdd[i][10].ToString().Replace('/', '-'));
                PlanPostRes.Add("planEndDate", PlanJAAdd[i][11].ToString().Replace('/', '-'));
                MesPlanRes.Add(PlanPostRes);
            }

            int bb = 0;
            int cc = 0;
            string UploadSuccess = "";
            string UploadFailure = "";
            foreach (var Item in MesPlanRes)
            {


                if (Item["code"].ToString() == "1")
                {
                    bb++;
                    UploadSuccess = UploadSuccess + "计划单号为[" + Item["planNumber"].ToString() + "]订单数量为[" + Item["planQty"].ToString() + "]订单号为[" + Item["orderNumber"].ToString() + "]计划开始时间为[" + Item["planStartDate"].ToString() + "]计划结束时间为[" + Item["planEndDate"].ToString() + "]上传至MES成功";

                }
                else
                {
                    cc++;
                    UploadFailure = UploadFailure + "计划单号为[" + Item["planNumber"].ToString() + "]订单数量为[" + Item["planQty"].ToString() + "]订单号为[" + Item["orderNumber"].ToString() + "]计划开始时间为[" + Item["planStartDate"].ToString() + "]计划结束时间为[" + Item["planEndDate"].ToString() + "]上传至MES失败" + "原因为：" + Item["msg"].ToString() + "\r\n";
                }

            }
            MessageBox.Show("共上传成功" + bb.ToString() + "条\r\n" + UploadSuccess + "\r\n共上传失败" + cc.ToString() + "条\r\n" + UploadFailure);


        }

        private void button5_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// 小诸葛取计划单列表
            /// </summary>
            //拼地址串
            url = gofServerAdd + "v1/GET/getPlanOrder";
            //创建一个字典类型数据来，传递参数

            parasMes.Clear();
            parasMes.Add("key", key);

            parasMes.Add("date", JHDateTimePicker.Text + " 00:00:00");
            parasMes.Add("indexPage", "1");
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, parasMes);
            JObject PlanJOMes = (JObject)JsonConvert.DeserializeObject(getJson);


            if (PlanJOMes["data"].Type != JTokenType.Null)
            {
                for (int i = 1; i <= int.Parse(PlanJOMes["data"][0]["pageCount"].ToString()); i++)
                {
                    parasMes.Clear();
                    parasMes.Add("key", key);
                    parasMes.Add("date", JHDateTimePicker.Text + " 00:00:00");
                    parasMes.Add("indexPage", i.ToString());
                    //传入HttpUitls类，并取得返回值
                    getJson = HttpUitls.Get(url, parasMes);
                    PlanJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
                    foreach (var item in PlanJOMes["data"][0]["plan"])
                    {
                        PlanJAMes.Add(item);
                    }

                }
            }
            getJson = PlanJAMes.ToString();
            /// < summary >
            /// 总结mes中需要添加的任务单号
            /// </ summary >
            JArray TaskJAAdd = new JArray();
            int d = 0;
            string awdc = PlanJAMes.ToString();
            string NotUpload = "";

            for (int i = 0; i < PlanJAKd.Count; i++)
            {
                    /// <summary>
                    /// 取MESlist中的moldId productId machineId panId
                    /// </summary>

                    int b = -1;
                    int c = -1;
                    int f = -1;
                    int g = -1;
                    
                    b = MouldJAMes.TakeWhile(x => x["moldNo"].ToString() != PlanJAKd[i][6].ToString()).Count();//判断第i个TaskJAKd的moldNo在MESMouldlist中第几个

                    c = ProductJAMes.TakeWhile(x => x["partnum"].ToString() != PlanJAKd[i][4].ToString()).Count();//判断第i个TaskJAKd的moldNo在MESproductlist中第几个
                    
                    f = MachineJAMes.TakeWhile(x => x["machineSN"].ToString() != PlanJAKd[i][12].ToString()).Count();//判断第i个TaskJAKd的moldNo在MESTasklist中第几个

                    g = PlanJAMes.TakeWhile(x => x["planNumber"].ToString() != (PlanJAKd[i][0].ToString() + PlanJAKd[i][1].ToString())).Count();//判断第i个TaskJAKd的moldNo在MESproductlist中第几个
                    //
                    if (b < MouldJAMes.Count() && c < ProductJAMes.Count() && f < MachineJAMes.Count() && g < PlanJAMes.Count())
                    {
                        TaskJAAdd.Add(PlanJAKd[i]);
                        //根据f(在meslist中的编号)把planId加到TaskJAAdd中
                        TaskJAAdd[d][0].AddAfterSelf(PlanJAMes[g]["planId"]);
                        //根据f(在meslist中的编号)把machineId加到TaskJAAdd中
                        TaskJAAdd[d][0].AddAfterSelf(MachineJAMes[f]["macId"]);
                        //根据b(在meslist中的编号)把moldid加到TaskJAAdd中
                        TaskJAAdd[d][0].AddAfterSelf(MouldJAMes[b]["moldId"]);
                        //根据b(在meslist中的编号)把productid加到TaskJAAdd中
                        TaskJAAdd[d][0].AddAfterSelf(ProductJAMes[c]["productId"]);
                        TaskJAAdd[d][5].Replace(  TaskJAAdd[d][0].ToString() + TaskJAAdd[d][5].ToString());//根据FTreeEntity_FSEQ的位置确定位置
                        d++;
                    }
                    else if (b >= MouldJAMes.Count())
                    {
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，模具号为" + PlanJAKd[i][6].ToString() + "的模具在MES的模具列表中不存在！" + "\r\n";
                        //解释说明未同步mould信息
                    }
                    else if (c >= ProductJAMes.Count())
                    { 
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，物料号为" + PlanJAKd[i][4].ToString() + "的物料在MES的物料列表中不存在！" + "\r\n";
                        //解释说明未同步product信息，需要先同步product信息

                    }
                    else if (f >= MachineJAMes.Count())
                    {
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，机台号为" + PlanJAKd[i][12].ToString() + "的机台在MES的机台列表中不存在！" + "\r\n";
                    }
                    else if (g >= PlanJAMes.Count())
                    {
                        NotUpload = NotUpload + PlanJAKd[i][0].ToString() + "中的第" + PlanJAKd[i][1].ToString() + "行，计划单号为" + PlanJAKd[i][0].ToString() + PlanJAKd[i][1].ToString() + "的计划单在MES的计划单列表中不存在！" + "\r\n";

                    }



            }
            //string aaaaaaaa = PlanJANotAdd.ToString();
            MessageBox.Show(NotUpload);
            url = gofServerAdd + "v1/POST/addTask";
            for (int i = 0; i < TaskJAAdd.Count; i++)
            {
                parasMes = new Dictionary<string, string>();
                parasMes.Add("key", key);
                parasMes.Add("planNumber", TaskJAAdd[i][5].ToString());
                parasMes.Add("planId", TaskJAAdd[i][4].ToString());
                parasMes.Add("machineId", TaskJAAdd[i][3].ToString());
                parasMes.Add("moldId", TaskJAAdd[i][2].ToString());
                parasMes.Add("taskQty", TaskJAAdd[i][11].ToString());
                parasMes.Add("output", TaskJAAdd[i][14].ToString());
                parasMes.Add("shift", TaskJAAdd[i][15].ToString() == "白班" ? "白班" : "晚班");
                parasMes.Add("planStartDate", TaskJAAdd[i][12].ToString().Replace('/', '-'));
                parasMes.Add("planEndDate", TaskJAAdd[i][13].ToString().Replace('/', '-'));
                parasMes.Add("productId", TaskJAAdd[i][1].ToString());
                string postJson = HttpUitls.Post(url, parasMes);
                //MessageBox.Show(postJson);//todo:验证成功，处理业务
                JObject TaskPostRes = (JObject)JsonConvert.DeserializeObject(postJson);
                //MesPlanRes
                TaskPostRes.Add("planNumber", TaskJAAdd[i][5].ToString());
                TaskPostRes.Add("taskQty", TaskJAAdd[i][11].ToString());
                TaskPostRes.Add("output", TaskJAAdd[i][14].ToString());
                TaskPostRes.Add("shift", TaskJAAdd[i][15].ToString() == "白班" ? "白班" : "晚班");
                TaskPostRes.Add("planStartDate", TaskJAAdd[i][12].ToString().Replace('/', '-'));
                TaskPostRes.Add("planEndDate", TaskJAAdd[i][13].ToString().Replace('/', '-'));
                MesTaskRes.Add(TaskPostRes);
            }
            int bb = 0;
            int cc = 0;
            string UploadSuccess = "";
            string UploadFailure = "";
            foreach (var Item in MesTaskRes)
            {


                if (Item["code"].ToString() == "1")
                {
                    bb++;
                    UploadSuccess = UploadSuccess + "任务单号为[" + Item["planNumber"].ToString() + "]任务数量为[" + Item["taskQty"].ToString() + "]单模产量为[" + Item["output"].ToString() + "]班次为[" + Item["shift"].ToString() + "]计划开始时间为[" + Item["planStartDate"].ToString() + "]计划结束时间为[" + Item["planEndDate"].ToString() + "]上传至MES成功";

                }
                else
                {
                    cc++;
                    UploadFailure = UploadFailure + "任务单号为[" + Item["planNumber"].ToString() + "]任务数量为[" + Item["taskQty"].ToString() + "]单模产量为[" + Item["output"].ToString() + "]班次为[" + Item["shift"].ToString() + "]计划开始时间为[" + Item["planStartDate"].ToString() + "]计划结束时间为[" + Item["planEndDate"].ToString() + "]上传至MES失败" + "原因为：" + Item["msg"].ToString() + "\r\n";
                }

            }
            MessageBox.Show("共上传成功" + bb.ToString() + "条\r\n" + UploadSuccess + "共上传失败" + cc.ToString() + "条\r\n" + UploadFailure);


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

            /// <summary>
            /// 小诸葛取任务单列表
            /// </summary>
            //拼地址串
            url = "";
            url = gofServerAdd + "v1/GET/getAllMacTaskList";
            //创建一个字典类型数据来，传递参数
            parasMes.Clear();
            parasMes.Add("key", key);
            parasMes.Add("state", "1");
            parasMes.Add("startTime", JHDateTimePicker.Text +" 00:00:00");
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, parasMes);
            JObject TaskJOMes = (JObject)JsonConvert.DeserializeObject(getJson);
            foreach (var item in TaskJOMes["data"])
            {
                if (item["totlaPage"].ToString() != "0")
                {
                    string abcd = item.ToString();
                    foreach (var taskList in item["taskList"])
                    {
                        //MessageBox.Show(taskList.ToString());
                        TaskJAMes.Add(taskList);

                    }
                }
                else 
                {
                    continue;
                }
            }

            /// <summary>
            /// 金蝶取生产汇报单信息
            /// </summary>
            HttpClient httpClient = new HttpClient();
            //服务地址
            httpClient.Url = "http://140.249.17.246:6888/K3Cloud/Kingdee.BOS.WebApi.ServicesStub.DynamicFormService.ExecuteBillQuery.common.kdsvc";
            //Json字串
            //把查询语句写入
            string data = "{\"FormId\":\"PRD_MORPT\",\"FilterString\":\"F_HBL_FProxyStartDate >= and F_HBL_FProxyStartDate FUSEORGID = '100019'and FFORBIDSTATUS = 'A' AND FDocumentStatus = 'C'\",\"FieldKeys\":\"FNAME,F_WW_MJMJDM,F_WW_MJMJZQS\"}";
            ParametersKd.Clear();
            ParametersKd.Add(data);
            //序列化上面数据返回结果
            httpClient.Content = JsonConvert.SerializeObject(ParametersKd);
            var result = httpClient.AsyncRequest();
            MouldJAKd = JArray.Parse(result);


            /// < summary >
            /// 小诸葛取报工单列表
            /// </ summary >
            //拼地址串
            url = gofServerAdd + "v1/GET/getAuditedProduceReportList";
            //创建一个字典类型数据来，传递参数

            parasMes.Clear();
            parasMes.Add("key", key);
            parasMes.Add("startDate", JHDateTimePicker.Text);
            parasMes.Add("endDate", JHDateTimePicker.Text);
            parasMes.Add("taskNo", "0086-2003050010");
            parasMes.Add("macId", "2c9480856f375c3f016f5fd1cdba7de5");
            parasMes.Add("indexPage", "1");
            parasMes.Add("tour", "白班");
            //传入HttpUitls类，并取得返回值
            getJson = HttpUitls.Get(url, parasMes);
            JObject PlanJOMes = (JObject)JsonConvert.DeserializeObject(getJson);


            
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //业务对象 FormId
            //string sFormId = " SAL_SaleOrder";  //销售订单
            //                                    //Data数据 
            //string sData = "{\"Ids\":\"\",\"Numbers\":[],\"RuleId\":\"\",\"TargetBillTypeId\":\"\",\"TargetOrgId\":\"0\",\"TargetFormId\":\"\",\"IsEnableDefaultRule\":\"false\",\"CustomParams\":{}} ";
            ////调用分配接口 
            //K3CloudApiClient client = new K3CloudApiClient("http://localhost:1200/");
            //client
            //var ret = client.Push(sFormId, sData);
        }
    }
}
