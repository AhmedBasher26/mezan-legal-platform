using Mezan.Domain.Entities;
using Mezan.Domain.Enums;

namespace Mezan.Application.Common;

public static class SeedDataBuilder
{
    public static string DocHtml(string title, string body) =>
        $@"<!DOCTYPE html>
<html dir=""rtl"" lang=""ar""><head><meta charset=""utf-8""><title>{title}</title>
<style>body{{font-family:'IBM Plex Sans Arabic',Tahoma,sans-serif;direction:rtl;padding:48px;line-height:2;color:#1a2433}}
h1{{text-align:center;border-bottom:3px double #a97e27;padding-bottom:12px;color:#0e2440}}
h2{{color:#224063;margin-top:28px}}p{{margin:10px 0;text-align:justify}}
.sig{{display:flex;justify-content:space-between;margin-top:80px}}
.box{{border:1px solid #c9d3e2;padding:16px 20px;background:#f7f9fc;border-radius:6px}}</style></head>
<body><h1>{title}</h1>{body}
<div class=""sig""><div>توقيع الطرف الأول<br/><br/>____________________</div>
<div>توقيع الطرف الثاني<br/><br/>____________________</div>
<div>توقيع المحامي<br/><br/>____________________</div></div></body></html>";

    public static (
        List<Client> clients,
        List<Case> cases,
        List<Hearing> hearings,
        List<TaskItem> tasks,
        List<Template> templates,
        List<CaseFile> caseFiles,
        List<Transaction> txs,
        List<CaseNote> notes,
        List<Activity> activities
    ) BuildDemoData(string lawyerId)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        const long h = 3600_000;
        var today = DateTime.UtcNow.Date;
        string D(int offset) => today.AddDays(offset).ToString("yyyy-MM-dd");

        var c1 = new Client { Id = "cl1_" + lawyerId, LawyerId = lawyerId, Name = "شركة الأفق للمقاولات", Phone = "0551112233", Email = "info@ofoq.sa", NationalId = "7001234567", Address = "الرياض — حي العليا، طريق الملك فهد", Notes = "عميل دائم منذ 2022، يفضل التواصل عبر البريد الرسمي.", CreatedAt = now - 90 * 24 * h };
        c1.ImportantDates.Add(new ImportantDate { Id = "id1_" + lawyerId, ClientId = c1.Id, Label = "تجديد السجل التجاري", Date = D(21) });

        var c2 = new Client { Id = "cl2_" + lawyerId, LawyerId = lawyerId, Name = "أحمد بن سعد الشمري", Phone = "0509876543", Email = "ahmed.shammari@mail.com", NationalId = "1045678901", Address = "جدة — حي الروضة", Notes = "", CreatedAt = now - 60 * 24 * h };
        var c3 = new Client { Id = "cl3_" + lawyerId, LawyerId = lawyerId, Name = "سارة محمد العتيبي", Phone = "0561234567", Email = "sara.otaibi@mail.com", NationalId = "1087654321", Address = "الرياض — حي النرجس", Notes = "تفضل الجلسات الصباحية.", CreatedAt = now - 41 * 24 * h };
        c3.ImportantDates.Add(new ImportantDate { Id = "id2_" + lawyerId, ClientId = c3.Id, Label = "انتهاء عقد الإيجار الحالي", Date = D(45) });

        var c4 = new Client { Id = "cl4_" + lawyerId, LawyerId = lawyerId, Name = "خالد عبدالله القحطاني", Phone = "0533334444", Email = "k.qahtani@mail.com", NationalId = "1023456789", Address = "الدمام — حي الشاطئ", Notes = "", CreatedAt = now - 25 * 24 * h };
        var c5 = new Client { Id = "cl5_" + lawyerId, LawyerId = lawyerId, Name = "نورة فهد السبيعي", Phone = "0577778888", Email = "noura.s@mail.com", NationalId = "1098765432", Address = "بريدة — حي الصفراء", Notes = "", CreatedAt = now - 15 * 24 * h };
        var c6 = new Client { Id = "cl6_" + lawyerId, LawyerId = lawyerId, Name = "مؤسسة النخبة التجارية", Phone = "0114567890", Email = "contact@nokhba.sa", NationalId = "7009876543", Address = "الرياض — حي الملقا", Notes = "تم إغلاق قضيتهم بنجاح.", CreatedAt = now - 120 * 24 * h };

        var clients = new List<Client> { c1, c2, c3, c4, c5, c6 };

        var cases = new List<Case>
        {
            new() { Id = "ca1_" + lawyerId, LawyerId = lawyerId, CaseNumber = "1042/2025", ClientId = c1.Id, Court = "المحكمة التجارية بالرياض", CaseType = "تجارية", FiledDate = D(-45), NextHearingDate = D(0), NextHearingTime = "10:30", Status = CaseStatus.Open, Description = "نزاع تجاري حول توريد مواد بناء وعدم الالتزام ببنود العقد المبرم بين الطرفين.", Notes = "", CreatedAt = now - 45 * 24 * h },
            new() { Id = "ca2_" + lawyerId, LawyerId = lawyerId, CaseNumber = "87/2025", ClientId = c2.Id, Court = "المحكمة العامة بجدة", CaseType = "عقارية", FiledDate = D(-38), NextHearingDate = D(0), NextHearingTime = "12:15", Status = CaseStatus.Open, Description = "دعوى إثبات ملكية عقار والمطالبة بإفراغ صك الملكية.", Notes = "", CreatedAt = now - 38 * 24 * h },
            new() { Id = "ca3_" + lawyerId, LawyerId = lawyerId, CaseNumber = "310/2025", ClientId = c3.Id, Court = "محكمة الأحوال الشخصية", CaseType = "أحوال شخصية", FiledDate = D(-30), NextHearingDate = D(1), NextHearingTime = "09:30", Status = CaseStatus.Postponed, Description = "دعوى حضانة ونفقة مع طلب زيارة دورية.", Notes = "تم التأجيل بناءً على طلب الطرفين لمحاولة الصلح.", CreatedAt = now - 30 * 24 * h },
            new() { Id = "ca4_" + lawyerId, LawyerId = lawyerId, CaseNumber = "522/2025", ClientId = c4.Id, Court = "المحكمة الجزائية المتخصصة", CaseType = "جنائية", FiledDate = D(-22), NextHearingDate = D(3), NextHearingTime = "11:00", Status = CaseStatus.Open, Description = "قضية جزائية — الدفاع عن الموكل في تهمة الاحتيال المالي مع إنكار كامل للتهمة.", Notes = "", CreatedAt = now - 22 * 24 * h },
            new() { Id = "ca5_" + lawyerId, LawyerId = lawyerId, CaseNumber = "764/2024", ClientId = c5.Id, Court = "المحكمة العمالية بالرياض", CaseType = "عمالية", FiledDate = D(-70), NextHearingDate = D(5), NextHearingTime = "13:30", Status = CaseStatus.Postponed, Description = "مطالبة بمستحقات نهاية الخدمة وبدل ساعات إضافية.", Notes = "", CreatedAt = now - 70 * 24 * h },
            new() { Id = "ca6_" + lawyerId, LawyerId = lawyerId, CaseNumber = "918/2024", ClientId = c6.Id, Court = "محكمة الاستئناف بالرياض", CaseType = "تجارية", FiledDate = D(-140), NextHearingDate = "", NextHearingTime = "", Status = CaseStatus.Closed, Description = "استئناف حكم تجاري — صدر الحكم النهائي لصالح الموكل.", Notes = "أُغلقت بعد كسب الاستئناف.", CreatedAt = now - 140 * 24 * h },
            new() { Id = "ca7_" + lawyerId, LawyerId = lawyerId, CaseNumber = "233/2025", ClientId = c2.Id, Court = "المحكمة الإدارية بالرياض", CaseType = "إدارية", FiledDate = D(-12), NextHearingDate = D(6), NextHearingTime = "10:00", Status = CaseStatus.Open, Description = "دعوى إلغاء قرار إداري صادر عن جهة حكومية.", Notes = "", CreatedAt = now - 12 * 24 * h }
        };

        var hearings = new List<Hearing>
        {
            new() { Id = "he1_" + lawyerId, LawyerId = lawyerId, CaseId = cases[0].Id, Date = D(-21), Time = "10:00", Type = "إثبات وتبادل مستندات", Notes = "تم تقديم كشف الحسابات.", CreatedAt = now - 21 * 24 * h },
            new() { Id = "he2_" + lawyerId, LawyerId = lawyerId, CaseId = cases[0].Id, Date = D(-6), Time = "11:30", Type = "تبادل مذكرات", Notes = "", CreatedAt = now - 6 * 24 * h },
            new() { Id = "he3_" + lawyerId, LawyerId = lawyerId, CaseId = cases[0].Id, Date = D(0), Time = "10:30", Type = "مرافعة", Notes = "إحضار أصل العقود والمرفقات.", CreatedAt = now - 5 * 24 * h },
            new() { Id = "he4_" + lawyerId, LawyerId = lawyerId, CaseId = cases[1].Id, Date = D(-15), Time = "09:00", Type = "نظر الدعوى", Notes = "", CreatedAt = now - 15 * 24 * h },
            new() { Id = "he5_" + lawyerId, LawyerId = lawyerId, CaseId = cases[1].Id, Date = D(0), Time = "12:15", Type = "نطق بالحكم", Notes = "", CreatedAt = now - 15 * 24 * h },
            new() { Id = "he6_" + lawyerId, LawyerId = lawyerId, CaseId = cases[2].Id, Date = D(1), Time = "09:30", Type = "جلسة صلح", Notes = "حضور الطرفين شخصياً.", CreatedAt = now - 4 * 24 * h },
            new() { Id = "he7_" + lawyerId, LawyerId = lawyerId, CaseId = cases[3].Id, Date = D(3), Time = "11:00", Type = "استجواب", Notes = "تجهيز أسئلة الدفاع.", CreatedAt = now - 3 * 24 * h },
            new() { Id = "he8_" + lawyerId, LawyerId = lawyerId, CaseId = cases[4].Id, Date = D(5), Time = "13:30", Type = "مرافعة ختامية", Notes = "", CreatedAt = now - 2 * 24 * h },
            new() { Id = "he9_" + lawyerId, LawyerId = lawyerId, CaseId = cases[5].Id, Date = D(-12), Time = "10:00", Type = "نطق بالحكم", Notes = "كسب الاستئناف.", CreatedAt = now - 12 * 24 * h },
            new() { Id = "he10_" + lawyerId, LawyerId = lawyerId, CaseId = cases[6].Id, Date = D(6), Time = "10:00", Type = "أولى", Notes = "", CreatedAt = now - 24 * h }
        };

        var tasks = new List<TaskItem>
        {
            new() { Id = "ta1_" + lawyerId, LawyerId = lawyerId, Title = "تجهيز مذكرة الدفاع — قضية 1042/2025", Description = "مراجعة بنود العقد وإعداد المذكرة الختامية قبل جلسة المرافعة.", Date = D(0), Time = "09:00", Priority = Priority.High, CaseId = cases[0].Id, ClientId = c1.Id, Completed = false, CreatedAt = now - 2 * 24 * h },
            new() { Id = "ta2_" + lawyerId, LawyerId = lawyerId, Title = "الاطلاع على تقرير الخبير العقاري", Description = "", Date = D(0), Time = "14:00", Priority = Priority.Medium, CaseId = cases[1].Id, ClientId = c2.Id, Completed = false, CreatedAt = now - 26 * h },
            new() { Id = "ta3_" + lawyerId, LawyerId = lawyerId, Title = "رفع مستندات القضية 87/2025", Description = "رفع الصكوك وكروكي العقار على منصة ناجز.", Date = D(0), Time = "08:30", Priority = Priority.High, CaseId = cases[1].Id, ClientId = c2.Id, Completed = true, CompletedAt = now - 3 * h, CreatedAt = now - 2 * 24 * h },
            new() { Id = "ta4_" + lawyerId, LawyerId = lawyerId, Title = "الاتصال بالموكل أحمد الشمري", Description = "تأكيد حضور جلسة الغد.", Date = D(0), Time = "16:30", Priority = Priority.Low, ClientId = c2.Id, Completed = false, CreatedAt = now - 20 * h },
            new() { Id = "ta5_" + lawyerId, LawyerId = lawyerId, Title = "سداد رسوم المحكمة الجزائية", Description = "", Date = D(-1), Time = "12:00", Priority = Priority.High, CaseId = cases[3].Id, ClientId = c4.Id, Completed = false, CreatedAt = now - 3 * 24 * h },
            new() { Id = "ta6_" + lawyerId, LawyerId = lawyerId, Title = "مراجعة مسودة اتفاق الصلح", Description = "مسودة مقدمة من الطرف الآخر.", Date = D(1), Time = "10:00", Priority = Priority.Medium, CaseId = cases[2].Id, ClientId = c3.Id, Completed = false, CreatedAt = now - 30 * h },
            new() { Id = "ta7_" + lawyerId, LawyerId = lawyerId, Title = "إعداد قائمة شهود الإثبات", Description = "", Date = D(3), Priority = Priority.High, CaseId = cases[3].Id, ClientId = c4.Id, Completed = false, CreatedAt = now - 10 * h }
        };

        const string saleBody = "<h2>البند الأول: أطراف العقد</h2><p>أبرم هذا العقد بين البائع والمشتري وفق البيانات المدونة أعلاه، ويعد التوقيع عليه إقراراً بصحة كافة البيانات.</p><h2>البند الثاني: المبيع</h2><p>محل هذا العقد هو المنقول/العقار الموصوف وصفاً نافياً للجهالة، بجميع ملحقاته وتوابعه.</p><h2>البند الثالث: الثمن وطريقة السداد</h2><div class=\"box\"><p>اتفق الطرفان على ثمن إجمالي وقدره (__________) ريال سعودي، يُسدد على دفعات وفق الجدول المتفق عليه.</p></div><h2>البند الرابع: التسليم</h2><p>يلتزم البائع بتسليم المبيع بالحالة المتفق عليها خلال (____) يوماً من توقيع هذا العقد.</p>";
        const string powBody = "<h2>أطراف التوكيل</h2><p>بموجب هذا التوكيل، يوكل الطرف الأول (الموكل) المحامي المذكور أعلاه في مباشرة جميع الإجراءات النظامية نيابةً عنه.</p><h2>نطاق التوكيل</h2><div class=\"box\"><p>يشمل التوكيل: الترافع أمام المحاكم على اختلاف درجاتها، تقديم المذكرات واللوائح، استلام الأحكام والصكوك، طلب التنفيذ، التنازل والصلح والإقرار وفق ما تقتضيه مصلحة الموكل.</p></div><h2>مدة التوكيل</h2><p>يسري هذا التوكيل لمدة عام ميلادي من تاريخ توثيقه ما لم يُلغَ كتابةً من الموكل.</p>";
        const string rentBody = "<h2>البند الأول: العين المؤجرة</h2><p>يؤجر الطرف الأول للطرف الثاني العقار الموصوف أعلاه لاستخدامه للغرض المتفق عليه فقط.</p><h2>البند الثاني: مدة الإيجار والأجرة</h2><div class=\"box\"><p>مدة هذا العقد عام كامل يبدأ من تاريخ (____) بأجرة سنوية قدرها (__________) ريال تُسدد مقدماً على دفعات ربع سنوية.</p></div><h2>البند الثالث: الالتزامات</h2><p>يلتزم المستأجر بالمحافظة على العين المؤجرة وسداد فواتير الخدمات، ولا يجوز له التأجير من الباطن دون موافقة خطية.</p>";
        const string ackBody = "<h2>موضوع الإقرار</h2><p>يقر الموقع أدناه بصحة الوقائع المبينة أعلاه إقراراً صريحاً لا رجوع فيه ولا إنكار له.</p><div class=\"box\"><p>وقد حُرر هذا الإقرار بحضور الشهود الموقعين أدناه، بعد أن تلا المقر مضمونه وتفهم أثره النظامي.</p></div>";

        var templates = new List<Template>
        {
            new() { Id = "tp1_" + lawyerId, LawyerId = lawyerId, Name = "عقد بيع", Ext = "doc", Size = 4820, Kind = TemplateKind.Builtin, Content = DocHtml("عقد بيع", saleBody), AddedAt = now - 40 * 24 * h },
            new() { Id = "tp2_" + lawyerId, LawyerId = lawyerId, Name = "توكيل عام", Ext = "doc", Size = 3940, Kind = TemplateKind.Builtin, Content = DocHtml("توكيل عام", powBody), AddedAt = now - 35 * 24 * h },
            new() { Id = "tp3_" + lawyerId, LawyerId = lawyerId, Name = "عقد إيجار", Ext = "doc", Size = 5110, Kind = TemplateKind.Builtin, Content = DocHtml("عقد إيجار", rentBody), AddedAt = now - 20 * 24 * h },
            new() { Id = "tp4_" + lawyerId, LawyerId = lawyerId, Name = "إقرار استلام", Ext = "doc", Size = 2180, Kind = TemplateKind.Builtin, Content = DocHtml("إقرار استلام", ackBody), AddedAt = now - 9 * 24 * h }
        };

        var caseFiles = new List<CaseFile>
        {
            new() { Id = "cf1_" + lawyerId, LawyerId = lawyerId, CaseId = cases[0].Id, Name = "توكيل عام", Ext = "doc", Size = 3940, Content = DocHtml("توكيل عام", powBody), TemplateId = templates[1].Id, AddedAt = now - 8 * 24 * h },
            new() { Id = "cf2_" + lawyerId, LawyerId = lawyerId, CaseId = cases[1].Id, Name = "صورة الصك", Ext = "pdf", Size = 812000, AddedAt = now - 5 * 24 * h }
        };

        var txs = new List<Transaction>
        {
            new() { Id = "tx1_" + lawyerId, LawyerId = lawyerId, ClientId = c1.Id, CaseId = cases[0].Id, Amount = 15000, Type = TransactionType.Fees, Date = D(-40), Notes = "دفعة أولى من الأتعاب", CreatedAt = now - 40 * 24 * h },
            new() { Id = "tx2_" + lawyerId, LawyerId = lawyerId, ClientId = c1.Id, CaseId = cases[0].Id, Amount = 10000, Type = TransactionType.Fees, Date = D(-10), Notes = "دفعة ثانية", CreatedAt = now - 10 * 24 * h },
            new() { Id = "tx3_" + lawyerId, LawyerId = lawyerId, ClientId = c1.Id, CaseId = cases[0].Id, Amount = 1200, Type = TransactionType.Expenses, Date = D(-8), Notes = "رسوم قيد الدعوى", CreatedAt = now - 8 * 24 * h },
            new() { Id = "tx4_" + lawyerId, LawyerId = lawyerId, ClientId = c2.Id, CaseId = cases[1].Id, Amount = 8000, Type = TransactionType.Fees, Date = D(-30), Notes = "أتعاب القضية", CreatedAt = now - 30 * 24 * h },
            new() { Id = "tx5_" + lawyerId, LawyerId = lawyerId, ClientId = c2.Id, CaseId = cases[1].Id, Amount = 450, Type = TransactionType.Expenses, Date = D(-12), Notes = "أجور خبير عقاري", CreatedAt = now - 12 * 24 * h },
            new() { Id = "tx6_" + lawyerId, LawyerId = lawyerId, ClientId = c3.Id, CaseId = cases[2].Id, Amount = 6000, Type = TransactionType.Fees, Date = D(-20), Notes = "", CreatedAt = now - 20 * 24 * h },
            new() { Id = "tx7_" + lawyerId, LawyerId = lawyerId, ClientId = c4.Id, CaseId = cases[3].Id, Amount = 12000, Type = TransactionType.Fees, Date = D(-15), Notes = "دفعة أولى", CreatedAt = now - 15 * 24 * h },
            new() { Id = "tx8_" + lawyerId, LawyerId = lawyerId, ClientId = c4.Id, CaseId = cases[3].Id, Amount = 2300, Type = TransactionType.Expenses, Date = D(-6), Notes = "رسوم وتقارير", CreatedAt = now - 6 * 24 * h },
            new() { Id = "tx9_" + lawyerId, LawyerId = lawyerId, ClientId = c6.Id, CaseId = cases[5].Id, Amount = 20000, Type = TransactionType.Fees, Date = D(-60), Notes = "أتعاب الاستئناف", CreatedAt = now - 60 * 24 * h },
            new() { Id = "tx10_" + lawyerId, LawyerId = lawyerId, ClientId = c6.Id, CaseId = cases[5].Id, Amount = 1500, Type = TransactionType.Expenses, Date = D(-50), Notes = "رسوم استئناف", CreatedAt = now - 50 * 24 * h }
        };

        var notes = new List<CaseNote>
        {
            new() { Id = "no1_" + lawyerId, LawyerId = lawyerId, CaseId = cases[0].Id, Text = "الطرف الآخر طلب مهلة إضافية لتقديم المستندات — تم الرفض شفوياً أمام القاضي.", CreatedAt = now - 6 * 24 * h },
            new() { Id = "no2_" + lawyerId, LawyerId = lawyerId, CaseId = cases[0].Id, Text = "التركيز على البند السابع من العقد في جلسة المرافعة القادمة.", CreatedAt = now - 2 * 24 * h },
            new() { Id = "no3_" + lawyerId, LawyerId = lawyerId, CaseId = cases[2].Id, Text = "الموكلية منفتحة على خيار الصلح بشرط جدولة النفقة.", CreatedAt = now - 24 * h }
        };

        var activities = new List<Activity>
        {
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Client, Text = "تمت إضافة موكل جديد: نورة فهد السبيعي", At = now - 15 * 24 * h },
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Case, Text = "تم إنشاء قضية جديدة برقم 233/2025", CaseId = cases[6].Id, At = now - 12 * 24 * h },
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Finance, Text = "تم تسجيل أتعاب 10,000 ر.س للقضية 1042/2025", CaseId = cases[0].Id, At = now - 10 * 24 * h },
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Template, Text = "تم رفع نموذج جديد: إقرار استلام", At = now - 9 * 24 * h },
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Hearing, Text = "تم تحديد جلسة مرافعة في القضية 1042/2025", CaseId = cases[0].Id, At = now - 5 * 24 * h },
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Task, Text = "تم إكمال مهمة: رفع مستندات القضية 87/2025", At = now - 3 * h },
            new() { Id = Guid.NewGuid().ToString("N"), LawyerId = lawyerId, Kind = ActivityKind.Note, Text = "تمت إضافة ملاحظة على القضية 310/2025", CaseId = cases[2].Id, At = now - 24 * h }
        };

        return (clients, cases, hearings, tasks, templates, caseFiles, txs, notes, activities);
    }
}
