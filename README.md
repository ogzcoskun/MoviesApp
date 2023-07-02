# MoviesApp

Bu uygulama kullanıcılara kayıt olarak bir film database'ine erişim sağlmakla beraber aynı zamanda bu filmler hakkında kişisel fikirlerini kayıt altında tutma ve dilediklerinde arkadaşları ile güven içinde paylaşma imkanı sunabilme amacı ile oluşturuluştur.

## Projenin Mimarisi:

Proje temel hatlarıyla 4 ana bölümden ve yazılım projesinden oluşmaktadır.

.DbFillerApp

.Accounts.Api

.Movies.Client.Api

.Movies.Admin.Api

Microservice dizayn pattern'a göre dizayn edilmiş bu projeler Event-Driven bir mimariye sahip. Bu mimari'nin tercih edilmesinin sebebi daha yüksek performanslı ve güvenli bir sistem ortaya çıkarıyor olmasıdır. Bunlara ek olarak da iş gruplarının olabildiğince küçültmeye olanak sağlaması ve böylelikle sistem yükünün hafiflemesi hedeflenmiştir.

### DbFillerApp
Bu Console uygulaması çalışmaya başladığı andan itibaren belirli aralıklarla "https://moviesdatabase.p.rapidapi.com" kaynağından 10'ar adet film bilgisi çekmekte ve bu bilgileri Database'e kaydetmektedir.

### Accounts.Api
Bu Api kullanıcılara Registration ve Login işlemlerini sağlamaktadır. Kullanıcılar kayıt işlemlerini tamamladıktan sonra login olarak kendilerine özel token'ı elde ederek geri kalan Api'lardaki işlemlerini güvenle gerçekleştirebilirler

### Movies.Client.Api
Bu Api kullanıcılara Database'deki filmleri görüntüleme, bu filmlere kendi yorumlarını ve puanlarını ekleme ve dilediği kişilere email yolu ile iletmelerini sağlamaktadır.

### Movies.Admin.Api
Bu Api herhangi bir Endpoint'e sahip olmaması ile birlikte diğer api'ların Database'lere kayıt işlemlerini ve email gönderme işlemlerini üstlenmektedir. diğer iki Api'ın publish etmiş olduğu Event'lere abone olarak tetiklenen ve gerekli yazma işlemlerini gerçekleştiren yapıdır.

## Kullanılan Teknolojiler:

- C# ve .Net Core 6
- 3 adet Web Api ve 1 adet Console Application.
- Docker: Containerization sağlamak için kullanılmıştır.
- MediatR: CQRS patter'in kullanıldığı yerlerde dağıtımı sağlamak amacı ile kullanılmıştır.
- RabbitMq ve Cap: Api'lar arası iletişimi sağlamak ve kurgulanan business logic gereksinimlerini sağlamak için kullanılmıştır.
- Entity Framework : ORM olarak tercih edilmiştir.
- Microsoft Identity ve JWT: Registration ve Token üretimi için tercih edilmiştir.
- Sql Server: Kullanıcı bilgilerini, Film Bilgilerini Hangfire Job bilgilerini ve Cap Publisher eventlerinin kayıtlarını tutmak için kullanılmıştır.
- Mongo Db: Kullanıcıların filmler hakkında yaptıkları yorumları ve bu filmlere verdikleri puanları tutmak için kullanılmıştır.
- Redis: Email ile iletilen film önerilerini Cache'lemek amacı ile kullanılmıştır.
- Hangfire: Belirli aralıklarla düzenli olarak çalışması gereken servisleri tetiklemek için kullanılmıştır.
- RestSharp: Uygulama içinden Http request'te bulunmak amacıyla kullanılmıştır.

## Nasıl Çalışıyor:
Bütün uygulamalar çalışmaya başladığında ilk olarak Database'in ve table'ların migration'ları Seeder'lar sayesinde tamamlanıyor. 

Kullanıcılar erişim sağlamak amacı ile öncelikle Accunts.Api üzerinden kişisel bilgileri ile birlikte kayıt oluyor ve bu kayıt bilgilerini kullanarak login işlemini gerçekleştiriyor.Kullanıcı kayıt işlemi gerçekleştirdiğinde UserRegisteredEvent publish ediliyor ve bunu Admin.Api yakalıyor ve Sql Server'a ek olarak MongoDb üzerinde kullanıcıya ait film yorum Entity'sini de yaratıyor . Login işleminde elde ettikleri token'ı saklayarak Client.Api'a geçiş sağlıyor.
- Registration:
{
  "fullName": "string",
  "email": "user@example.com",
  "password": "string",
  "confirmPassword": "string"
}

- Login:
{
  "email": "user@example.com",
  "password": "string"
}




Client.Api 5 tane Endpoint barındırıyor. Yukarıda elde ettiğimiz token ile bu api'a otorize oluyoruz(Başına Bearer eklenmeli!!!)
- GetAllMoviesPaged: PageNumber ve PageSize olmak üzere 2 adet Parametre ile çalışmaktadır(Boş bırakılma durumunda Default olarak 1 ve 10 olarak belirlenmiştir). Belirtilen sayfa ve ebatta Film bilgisi dönmektedir. (Kullanıcının UserId'si token içinden ayıklanmaktadır)

    PageNumber: Int ,       PageSize: Int

- GetMovieWithId: Parametre olarak movieId alır ve ilgili film bilgilerini döner. Aynı zamanda MongoDb üzerinden kullanıcı bu filme herhangi bir yorum ve puanlama yaptı ise bu bilgileri de döner.

    movieId: String

- PostComment: Parametre olarak movieId, rating, comment almaktadır. Kullanıcı yorum yapmak istediği film'in id'sini sağlar filme 0-10 arasında bir puan verir ve dilediği yorumu yazar.

  Bu bilgiler MongoDb üzerinde kullanıcının Yorumlar listesine eklenir. Bu sırada verdiği puan filmin puanına eklenir ve puanın ortalaması tekrar alınarak Sql Server üzerinde düzenlenir.

  {
    "movieId": "string",
    "rating": 10,
    "comment": "string"
  }

- RecommendAMovie: Parametre olarak toEmail, movieId ve personalMessage alınır. Api ilgili filmin bilgilerini ve Email'in iletileceği adresi ve kişisel mesajını içeren bir event publish eder. Bu Event Admin.api tarafından yakalanır ve istenen adrese Email olarak iletilir. Bunun ardından bu gönderim ve bilgileri redis üzerinde cache'lenir.

  toEmail: string  ,   movieId: string ,  personalMessage: string

- GetMyRecommendations: Herhangi bir Parametre almaz. Kullanıcının yapmış olduğu paylaşımları Redis üzerinden çeker ve kullanıcıya iletir.
