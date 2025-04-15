# Banking System

### მოკლე აღწერა

პროექტის მიზანია შეიქმნას საბანკო სისტემისთვის API, რომლის გამოყენებითაც მომხმარებლები შეძლებენ თანხების გადარიცხვას საკუთარ ან სხვა ანგარიშებზე და ATM-ით სარგებლობას.

სისტემის მენეჯერები კი შეძლებენ სხვადადასხვა ტიპის რეპორტების ნახვას.

პროექტი მოიცავს რამდენიმე მოდულს. თითოეული მოდული შეგვიძლია განვიხილოთ როგორც დამოუკიდებელი სისტემა, თუმცა ზოგიერთი მოდული დამოკიდებულია სხვა მოდულების არსებობაზე. ამიტომ მათი იმპლემენტაცია უნდა მოხდეს თანმიმდევრობით.

### მოდულები

- ინტერნეტბანკი
    - ოპერატორი
    - მომხმარებელი
- ATM კლიენტი
    - ბარათის ავტორიზაცია
    - ATM ოპერაციები
- რეპორტები
    - მომხმარებლების სტატისტიკა
    - ტრანზაქციების სტატისტიკა

### ინტერნეტბანკი

ინტერნეტბანკი უნდა იყოს ვებ აპლიკაცია, რომლის საშუალებითაც ბანკის ოპერატორებს შეეძლებათ მომხმარებლების რეგისტრაცია, მათთვის საბანკო ანგარიშების შექმნა და ანაგირშებზე ბარათების დამატება.

**ოპერატორი**

ოპერატორს უნდა შეეძლოს ფიზიკური პირების რეგისტრაცია, რომლის დროსაც შეიყვანს შემდეგ მონაცემებს:

- სახელი *
- გვარი *
- პირადი ნომერი *
- დაბადების თარიღი *
- ელ. ფოსტა *
- პაროლი *

მომხმარებლისთვის საბანკო ანგარიშის შექმნის დროს, ოპერატორს უნდა შეეძლოს მიუთითოს შემდეგი მონაცემები:

- IBAN (რეგისტრაციის დროს უნდა მოხდეს IBAN-ის ვალიდაცია. არავალიდური IBAN-ით ოპერატორმა ვერ უნდა შეძლოს ანგარიშის შექმნა)
- Amount (ანგარიშზე არსებული თანხა)
- Currency (ვალუტა. უნდა იყოს ასარჩევი ველი. შესაძლო მნიშვნელობები: GEL, USD, EUR)

ბარათის რეგისტრაციის დროს ოპერატორმა უნდა მიუთითოს შემდეგი მონაცემები:

- ბარათის ნომერი
- სახელი და გვარი
- ბარათის ვადა (წელი, თვე)
- 3 ნიშნა CVV კოდი (ონლაინ გადახდებისთვის)
- 4 ნიშნა PIN კოდი (ATM-დან თანხის გამოსატანად)

**მომხმარებელი**

დარეგისტრირებულ მომხმარებლებს უნდა შეეძლოთ ოპერატორის მიერ მათთვის შექმნილი ანგარიშების და ბარათების ნახვა. 

ინტერნეტბანკიდან მომხმარებელს უნდა შეეძლოს ორი ტიპის ტრანზაქციის შესრულება:

- საკუთარ ანგარიშებს შორის თანხის გადარიცხვა, რომლის დროსაც გადარიცხვის საკომისიო იქნება 0%
- ბანკის სხვა ანგარიშზე გადარიცხვა, რომლის დროსაც გადარიცხვის საკომისიო იქნება 1% + 0.5 (ლარი/დოლარი/ევრო)

თანხის გადარიცხვისას გათვალისწინებული უნდა იყოს ანგარიშების ვალუტების კურსები. თუ ერთი ანგარიშის ვალუტა განსხვავდება მეორე ანგარიშის ვალუტისგან, უნდა მოხდეს თანხის კონვერტაცია წინასწარ განსაზღვრული კურსის მიხედვით.

### ATM კლიენტი

ATM კლიენტისთვის საჭიროა შეიქმნას API, სადაც მომხმარებელს ბარათის ავტორიზაციის შემდეგ შეეძლება სხვადასხვა ოპერაციის ჩატარება.

**ბარათის ავტორიზაცია**

ATM-ის ნებისმიერი ოპერაციის ჩასატარებლად საჭიროა ასევე მოხდეს ბარათის ავტორიზაცია.

ამისათვის მომხმარებელმა უნდა მიუთითოს ბარათის ნომერი და PIN კოდი.

ავტორიზაცია არ უნდა იყოს წარმატებული, თუ ბარათი ვადაგასულია.

**ATM ოპერაციები**

ავტორიზაციის შემდეგ, შესაძლებელი უნდა იყოს შემდეგი ოპერაციების ჩატარება:

- ბალანსის ნახვა
- თანხის გამოტანა GEL, USD ან EUR ვალუტაში
- პინ კოდის შეცვლა

ATM-დან თანხის გამოტანის საკომისიო უნდა იყოს 2% და 24 საათის განმავლობაში შესაძლებელი უნდა იყოს მაქსიმუმ 10,000 ლარის გამოტანა.

### რეპორტები

ბანკის მენეჯერებს უნდა შეეძლოთ შემდეგი ტიპის რეპორტების ნახვა (API-მ უნდა დააბრუნოს შედეგები JSON ფორმატში):

- მომხმარებლების სტატისტიკა
    - მიმდინარე წელს დარეგისტრირებული მომხმარებლების რაოდენობა
    - ბოლო ერთი წლის განმავლობაში დარეგისტრირებული მომხმარებლების რაოდენობა
    - ბოლო 30 დღეში დარეგისტრირებული მომხმარებლების რაოდენობა
- ტრანზაქციების სტატისტიკა
    - ბოლო 1 თვეში/6 თვეში/1წელში განხორციელებული ტრანზაქციების რაოდენობა
    - ბოლო 1 თვეში/6 თვეში/1წელში ტრანზაქციებიდან მიღებული შემოსავალის მოცულობა (ლარში/დოლარში/ევროში)
    - საშუალოდ ერთი ტრანზაქციიდან მიღებული შემოსავალი (ლარში/დოლარში/ევროში)
    - ბოლო ერთ თვეში ტრანზაქციების რაოდენობა დღეების მიხედვით (ჩარტი)
    - ATM-დან გამოტანილი თანხის ჯამური რაოდენობა
 
 ![image](https://github.com/user-attachments/assets/7cec56ab-e5d9-46d1-9985-c5dd565d0024)


