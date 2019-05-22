using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JiebaNet.Analyser;
using JiebaNet.Segmenter;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using PostMatch.Api.Models;
using PostMatch.Core.Entities;
using Resume = PostMatch.Api.Models.Resume;

namespace PostMatch.Api.Helpers
{
    public class Calculate
    {
//        private double getSimilarity(string doc1, string doc2)
//        {
//            if (doc1 != null && doc1.Trim().Length > 0 && doc2 != null
//                && doc2.Trim().Length > 0)
//            {
//                Dictionary<int, int[]> AlgorithmMap = new Dictionary<int, int[]>();
//                //将两个字符串中的中文字符以及出现的总数封装到，AlgorithmMap中
//                for (int i = 0; i < doc1.Length; i++)
//                {
//                    char d1 = doc1.ToCharArray()[i];
//                    if (isHanZi(d1))
//                    {
//                        int charIndex = getGB2312Id(d1);
//                        if (charIndex != -1)
//                        {
//                            int[] fq = null;
//                            try
//                            {
//                                fq = AlgorithmMap[charIndex];
//                            }
//                            catch (Exception)
//                            {
//                            }
//                            finally
//                            {
//                                if (fq != null && fq.Length == 2)
//                                {
//                                    fq[0]++;
//                                }
//                                else
//                                {
//                                    fq = new int[2];
//                                    fq[0] = 1;
//                                    fq[1] = 0;
//                                    AlgorithmMap.Add(charIndex, fq);
//                                }
//                            }
//                        }
//                    }
//                }
//
//                for (int i = 0; i < doc2.Length; i++)
//                {
//                    char d2 = doc2.ToCharArray()[i];
//                    if (isHanZi(d2))
//                    {
//                        int charIndex = getGB2312Id(d2);
//                        if (charIndex != -1)
//                        {
//                            int[] fq = null;
//                            try
//                            {
//                                fq = AlgorithmMap[charIndex];
//                            }
//                            catch (Exception)
//                            {
//                            }
//                            finally
//                            {
//                                if (fq != null && fq.Length == 2)
//                                {
//                                    fq[1]++;
//                                }
//                                else
//                                {
//                                    fq = new int[2];
//                                    fq[0] = 0;
//                                    fq[1] = 1;
//                                    AlgorithmMap.Add(charIndex, fq);
//                                }
//                            }
//                        }
//                    }
//                }
//
//                double sqdoc1 = 0;
//                double sqdoc2 = 0;
//                double denominator = 0;
//                foreach (KeyValuePair<int, int[]> par in AlgorithmMap)
//                {
//                    int[] c = par.Value;
//                    denominator += c[0] * c[1];
//                    sqdoc1 += c[0] * c[0];
//                    sqdoc2 += c[1] * c[1];
//                }
//
//                return denominator / Math.Sqrt(sqdoc1 * sqdoc2);
//            }
//            else
//            {
//                return 0;
//            }
//        }
//
//        private static bool isHanZi(char ch)
//        {
//            // 判断是否汉字
//            return (ch >= 0x4E00 && ch <= 0x9FA5);
//        }
//
//        /**
//         * 根据输入的Unicode字符，获取它的GB2312编码或者ascii编码，
//         * 
//         * @param ch
//         *            输入的GB2312中文字符或者ASCII字符(128个)
//         * @return ch在GB2312中的位置，-1表示该字符不认识
//         */
//        private static short getGB2312Id(char ch)
//        {
//            try
//            {
//                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
//                var buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(ch.ToString());
//                if (buffer.Length != 2)
//                {
//                    // 正常情况下buffer应该是两个字节，否则说明ch不属于GB2312编码，故返回'?'，此时说明不认识该字符
//                    return -1;
//                }
//
//                var b0 = (int) (buffer[0] & 0x0FF) - 161; // 编码从A1开始，因此减去0xA1=161
//                var b1 = (int) (buffer[1] & 0x0FF) - 161; // 第一个字符和最后一个字符没有汉字，因此每个区只收16*6-2=94个汉字
//                return (short) (b0 * 94 + b1);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//
//            return -1;
//        }

        private List<Recommend> matchingResults = new List<Recommend>();
        
        //根据岗位对象获得最合适的前五个简历
        public List<Recommend> GetMatchingResultsByPost(List<Resume> resumes, Post post)
        {
            matchingResults.Clear();
            //获得post的需要测试的所有tf idf
            var postMap = GetPostTf_Idf(post);
            foreach (var resume in resumes)
            {
                var result =  getMatchingResultByPost(resume, postMap);
                matchingResults.Add(result);
            }

            var i = 0;
            matchingResults.Sort((r1, r2) => r2.RecommendNumber.CompareTo(r1.RecommendNumber));

            var relayResults = new List<Recommend>();
            
            foreach (var result in matchingResults)
            {
                i++;
                if (i > 5)
                {
                    break;
                }
                result.CompanyId = post.CompanyId;
                result.PostId = post.PostId;
                relayResults.Add(result);
            }

            return relayResults;
        }
        
        //获得一个岗位和一个简历的匹配程度
        private Recommend getMatchingResultByPost(Resume resume, Dictionary<string,IEnumerable<WordWeightPair>> post)
        {
            var recommend = new Recommend();
            
            if(resume == null || post == null) return recommend;
            
            //获得地点是否匹配
            var addr = Similarity(resume.familyAddress + resume.resumeWorkPlace, post["addr"]);
            addr = addr > 0 ? addr : 0;
//            Console.Write("1");
            // 获得职位是否匹配
            var job = Similarity(resume.resumePostName, post["PostName"]);
            job = job > 0 ? job : 0;
//            Console.Write("2");
            //获得工作年限匹配
            var exTime = Similarity(resume.workYear, post["PostExperience"]);
            exTime = exTime > 0 ? exTime : 0;
//            Console.Write("3");
            //获得岗位职责和个人信息匹配度
            var info = Similarity(resume.resumeExperience, post["PostDescription"]);
            info = info > 0 ? info : 0;
//            Console.Write("4");
            //技能匹配度
            var skill = Similarity(resume.skill, post["PostDescription"]);
            skill = skill > 0 ? skill : 0;
//            Console.Write("5");
            //学历匹配度
            var academic = Similarity(resume.academic, post["AcademicRequirements"]);
            academic = academic > 0 ? academic : 0;
//            Console.Write("6");
            //最终匹配结果
            var result = addr + exTime + academic + job * 2 + info * 2 + skill * 3.5;
            
            recommend.ResumeId = resume.resumeId;
            recommend.RecommendNumber = result;
            
            return recommend;
        }
        
        //获得post的需要测试的所有tf idf
        private Dictionary<string,IEnumerable<WordWeightPair>> GetPostTf_Idf(Post post)
        {
            var map = new Dictionary<string,IEnumerable<WordWeightPair>>();
            //获得地点
            map["addr"] = extractor.ExtractTagsWithWeight(post.City + post.PostWorkPlace,200);
            //获得职位
            map["PostName"] = extractor.ExtractTagsWithWeight(post.PostName,200);
            //获得工作年限
            map["PostExperience"] = extractor.ExtractTagsWithWeight(post.PostExperience,200);
            //获得岗位职责和个人信息
            map["PostDescription"] = extractor.ExtractTagsWithWeight(post.PostDescription,200);
            //技能
            //学历
            map["AcademicRequirements"] = extractor.ExtractTagsWithWeight(post.AcademicRequirements,200);
            return map;
        }
        
        
        //根据简历对象获得最合适的前五个岗位
        public List<Recommend> GetMatchingResultsByResume(Resume resume,List<Post> posts)
        {
            matchingResults.Clear();
            //获得post的需要测试的所有tf idf
            var resumeMap = GetResumeTf_Idf(resume);
            foreach (var post in posts)
            {
                var result =  getMatchingResultByResume(post, resumeMap);
                matchingResults.Add(result);
            }

            var i = 0;
            matchingResults.Sort((r1, r2) => r2.RecommendNumber.CompareTo(r1.RecommendNumber));

            var relayResults = new List<Recommend>();
            
            foreach (var result in matchingResults)
            {
                i++;
                if (i > 5)
                {
                    break;
                }
                result.ResumeId = resume.resumeId;
                relayResults.Add(result);
            }

            return relayResults;
        }
        
        //获得一个resume和一个post的匹配程度
        private Recommend getMatchingResultByResume(Post post, Dictionary<string,IEnumerable<WordWeightPair>> resume)
        {
            var recommend = new Recommend();
            
            if(resume == null || post == null) return recommend;
            
            //获得地点是否匹配
            var addr = Similarity(post.City + post.PostWorkPlace, resume["addr"]);
            addr = addr > 0 ? addr : 0;
//            Console.Write("1");
            // 获得职位是否匹配
            var job = Similarity(post.PostName, resume["resumePostName"]);
            job = job > 0 ? job : 0;
//            Console.Write("2");
            //获得工作年限匹配
            var exTime = Similarity(post.PostExperience, resume["workYear"]);
            exTime = exTime > 0 ? exTime : 0;
//            Console.Write("3");
            //获得岗位职责和个人信息匹配度
            var info = Similarity(post.PostDescription, resume["resumeExperience"]);
            info = info > 0 ? info : 0;
//            Console.Write("4");
            //技能匹配度
            var skill = Similarity(post.PostDescription, resume["skill"]);
            skill = skill > 0 ? skill : 0;
//            Console.Write("5");
            //学历匹配度
            var academic = Similarity(post.AcademicRequirements, resume["academic"]);
            academic = academic > 0 ? academic : 0;
//            Console.Write("6");
            //最终匹配结果
            var result = addr + exTime + academic + job * 2 + info * 2 + skill * 3.5;
            
            recommend.CompanyId = post.CompanyId;
            recommend.PostId = post.PostId;
            recommend.RecommendNumber = result;
            
            return recommend;
        }
        
        
        //获得resume的需要测试的所有tf——idf
        private Dictionary<string,IEnumerable<WordWeightPair>> GetResumeTf_Idf(Resume resume)
        {
            var map = new Dictionary<string,IEnumerable<WordWeightPair>>();
            //获得地点
            map["addr"] = extractor.ExtractTagsWithWeight(resume.familyAddress + resume.resumeWorkPlace,200);
            //获得职位
            map["resumePostName"] = extractor.ExtractTagsWithWeight(resume.resumePostName,200);
            //获得工作年限
            map["workYear"] = extractor.ExtractTagsWithWeight(resume.workYear,200);
            //获得岗位职责和个人信息
            map["resumeExperience"] = extractor.ExtractTagsWithWeight(resume.resumeExperience,200);
            //技能
            map["skill"] = extractor.ExtractTagsWithWeight(resume.skill,200);
            //学历
            map["academic"] = extractor.ExtractTagsWithWeight(resume.academic,200);
            return map;
        }
        
       
        /*=====================================================*/
        

        /*//根据岗位对象获得最合适的前五个简历
        public List<Recommend> GetMatchingResultsByPost(List<Resume> resumes, Post post)
        {
            matchingResults.Clear();
            foreach (var resume in resumes)
            {
                Console.Write("--获取单个简历--");
                //获得post的需要测试的所有tf idf
                
                var  result =  getMatchingResult(resume, post);
                matchingResults.Add(result);
            }

            var i = 0;
            matchingResults.Sort((r1, r2) => r2.RecommendNumber.CompareTo(r1.RecommendNumber));

            var relayResults = new List<Recommend>();
            
            foreach (var result in matchingResults)
            {
                i++;
                if (i > 5)
                {
                    break;
                }
                relayResults.Add(result);
            }

            return relayResults;
        }
        
        //根据简历对象获得最合适的前五个岗位
        public List<Recommend> GetMatchingResultsByResume(Resume resume, List<Post> posts)
        {
            matchingResults.Clear();
            foreach (var post in posts)
            {
                var  result =  getMatchingResult(resume, post);
                matchingResults.Add(result);
            }

            Console.Write("2 ");
            var i = 0;
            matchingResults.Sort((r1, r2) => r2.RecommendNumber.CompareTo(r1.RecommendNumber));

            var relayResults = new List<Recommend>();
            
            foreach (var result in matchingResults)
            {
                i++;
                if (i > 5)
                {
                    break;
                }
                relayResults.Add(result);
                
            }

            return relayResults;
        }*/
        
       
        
        /*//获得一个岗位和一个简历的匹配程度
        private Recommend getMatchingResult(Resume resume, Post post)
        {
            var recommend = new Recommend();
            
            if(resume == null || post == null) return recommend;
            
            //获得地点是否匹配
            ArrayList companyAddrs = new ArrayList();
            ArrayList userAddrs = new ArrayList();
            companyAddrs.Add(post.City);
            companyAddrs.Add(post.PostWorkPlace);
            userAddrs.Add(resume.resumeWorkPlace);
            userAddrs.Add(resume.familyAddress);
            var addr = CitySame(userAddrs, companyAddrs);
            Console.Write("1");
            // 获得职位是否匹配
            var job = JobSame(post.PostName, resume.resumePostName);
            Console.Write("2");
            //获得工作年限匹配
            var exTime = ExperienceSame(post.PostExperience, resume.workYear);
            Console.Write("3");

            //获得岗位职责和个人信息匹配度
            var info = InfoSame(post.PostDescription, resume.resumeExperience + resume.academic);
            Console.Write("4");

            //技能匹配度
            var skill = SkillSame(post.PostDescription, resume.skill);
            Console.Write("5");

            //学历匹配度
            var academic = AcademicSame(post.AcademicRequirements, resume.academic);
            Console.Write("6");

            //最终匹配结果
            var result = addr + exTime + academic + job * 2 + info * 2 + skill * 3.5;
            
            recommend.CompanyId = post.CompanyId;
            recommend.PostId = post.PostId;
            recommend.ResumeId = resume.resumeId;
            recommend.RecommendNumber = result;
            
            return recommend;
        }*/


        /*//获得城市是否匹配
        private double CitySame(ArrayList companyAddrs, ArrayList userAddrs)
        {
            if (companyAddrs == null) throw new ArgumentNullException(nameof(companyAddrs));
            if (userAddrs == null) throw new ArgumentNullException(nameof(userAddrs));
            string a = "";
            string b = "";
            //ArrayList l1=new ArrayList();
            //l1.Add(companyAddrs);
            //l1.Add(userAddrs);
            string[] text = new string[2];
            foreach (var companyAddr in companyAddrs)
            {
                a += companyAddr;
            }
            foreach (var userAddr in userAddrs)
            {
                b += userAddr;
            }

            text[0] = a;
            text[1] = b;

            return SimilarityTest(text);
            
            //            return getSimilarity(companyJob,userJob);
        }

        //职位是否匹配
        private double JobSame(string companyJob, string userJob)
        {
            ArrayList l2=new ArrayList();
            l2.Add(companyJob);
            l2.Add(userJob);
            string[] text = (string[])l2.ToArray(typeof( string)) ;
            
//            return getSimilarity(companyJob,userJob);
            return SimilarityTest(text);

        }
        
        // 岗位描述和个人经验+专业匹配度
        private double InfoSame(string companyInfo, string userInfo)
        { 
            ArrayList l3=new ArrayList();
            l3.Add(companyInfo);
            l3.Add(userInfo);
            string[] text = (string[])l3.ToArray(typeof( string)) ;
            return SimilarityTest(text);

//            return getSimilarity(companyInfo, userInfo);
        }
        
        //技能匹配度
        private double SkillSame(string companyInfo, string userSkill)
        {
            ArrayList l4=new ArrayList();
            l4.Add(companyInfo);
            l4.Add(userSkill);
            string[] text = (string[])l4.ToArray(typeof( string)) ;
            return SimilarityTest(text);
            
//            return getSimilarity(companyInfo, userSkill);
        }
        
        //学历匹配度
        private double AcademicSame(string companyA, string userA)
        {
            ArrayList l5=new ArrayList();
            l5.Add(companyA);
            l5.Add(userA);
            string[] text = (string[])l5.ToArray(typeof( string)) ;
            return SimilarityTest(text);
            
//            return getSimilarity(companyA, userA);
        }

        string pattern = @"(\d*)?";

        //经验是否匹配
        private double ExperienceSame(string comExTime, string userExTime)
        {
            double result = 0.0;

            if (comExTime.Contains("不限"))
            {
                return result + 0.7;
            }
            
            ArrayList l6=new ArrayList();
            l6.Add(comExTime);
            l6.Add(userExTime);
            string[] text = (string[])l6.ToArray(typeof( string)) ;
//            string[] text += comExTime;
            
            return SimilarityTest(text);
//            return getSimilarity(comExTime,userExTime);
        }*/
        
        private static TfidfExtractor extractor = new TfidfExtractor();

        private static string pattern = "\\d*|[a-zA-Z]*";
        
        public static double Similarity(string s1, IEnumerable<WordWeightPair> keywords2)
        {
            var keywords1 = extractor.ExtractTagsWithWeight(s1,200);
            Dictionary<string, double> map = new Dictionary<string, double>();
            //将两个文本的关键词key集合合并到一个map中
            foreach (var keyword in keywords1)
            {
                map.Add(keyword.Word, 0);
//                Console.WriteLine("{0}:{1}", keyword.Word, keyword.Weight);
            }
            foreach (var keyword in keywords2)
            {
                map[keyword.Word] = 0;
//                Console.WriteLine("{0}:{1}", keyword.Word, keyword.Weight);
            }
            //分别将两个关键词集合存入map中
            Dictionary<string, double> map1 = new Dictionary<string, double>();
            Dictionary<string, double> map2 = new Dictionary<string, double>();
            foreach (var key in map.Keys)
            {
                map1.Add(key, 0);
            }
            foreach (var key in map.Keys)
            {
                map2.Add(key, 0);
            }
            
            foreach (var keyword in keywords1)
            {
                Match match = Regex.Match(keyword.Word, pattern);
//                Console.WriteLine("{0} : {1}",match.Length,match.Value);
                if (match.Length > 1)
                {
                    map1[keyword.Word] = keyword.Weight * 3;
                }
                else
                {
                    map1[keyword.Word] = keyword.Weight;
                }

            }
            foreach (var keyword in keywords2)
            {
                map2[keyword.Word] = keyword.Weight;
            }
            // 只获得map中的所有值
            List<double> list1 = new List<double>();
            List<double> list2 = new List<double>();
            foreach (var key in map1.Keys)
            {
                list1.Add(map1[key]);
//                Console.WriteLine("{0}:{1}", key, map1[key]);
            }
//            Console.WriteLine("");
//            Console.WriteLine("");
//            Console.WriteLine("");
            foreach (var key in map2.Keys)
            {
                list2.Add(map2[key]);
//                Console.WriteLine("{0}:{1}", key, map2[key]);
            }

            //计算内积
            double sum = 0;
            for (int m = 0; m < list1.Count; m++)
            {
                if (m >= list2.Count)
                {
                    break;
                }
                sum += list1[m] * list2[m];
            }
            double i_length = 0;
            double j_length = 0;
            //第i份文档的向量模长
            for (int n = 0; n < list1.Count; n++)
            {
                i_length += list1[n] * list1[n];
            }
            i_length = Math.Sqrt(i_length);
            // 第j份文档的向量模长
            for (int n = 0; n < list2.Count; n++)
            {
                j_length += list2[n] * list2[n];
            }
            j_length = Math.Sqrt(j_length);
            //夹角余弦值计算公式，两向量内积除以两向量的模长乘积
            long c = DateTime.Now.Millisecond;
            return sum / (i_length * j_length);
        }
        
        //计算工作性质是否是一个工作性质
        private static double IsSame(string a, string b)
        {
            const double result = 1.0;
            if (a.Equals(b))
            {
                return result;
            }

            if (a.Contains(b) || b.Contains(a))
            {
                return result - 0.5;
            }
            else
            {
                return 0;
            }
        }
    }
}