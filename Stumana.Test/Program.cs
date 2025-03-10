//using Microsoft.EntityFrameworkCore;
//using Stumana.DataAccess.Services;
//using Stumana.DataAcess.Data;
//using Stumana.DataAcess.Models;

//using (var context = AppDbContextFactory.Instance.CreateDbContext())
//{

//    context.Database.Migrate();

//    var province = new Province { Id = "Province01", Name = "Province A" };
//    await GenericDataService<Province>.Instance.CreateOneAsync(province);

//    var district = new District { Id = "District01", Name = "District A", ProvinceId = province.Id };
//    await GenericDataService<District>.Instance.CreateOneAsync(district);

//    var school = new School { Id = "School01", Name = "High School A", Rules = "Standard Rules", IsRegistered = true, DistrictId = district.Id };
//    await GenericDataService<School>.Instance.CreateOneAsync(school);

//    var grade = new Grade { Id = "Grade01", Level = 10, SchoolId = school.Id };
//    await GenericDataService<Grade>.Instance.CreateOneAsync(grade);

//    var classroom = new Classroom { Id = "Classroom01", Name = "Class A", Capacity = 30, GradeId = grade.Id };
//    await GenericDataService<Classroom>.Instance.CreateOneAsync(classroom);

//    var schoolYear = new SchoolYear { Id = "SchoolYear01", StartYear = 2025, SchoolId = school.Id };
//    await GenericDataService<SchoolYear>.Instance.CreateOneAsync(schoolYear);

//    var classroomOffering = new ClassroomOffering { Id = "ClassroomOffering01", ClassroomId = classroom.Id, SchoolYearId = schoolYear.Id };
//    await GenericDataService<ClassroomOffering>.Instance.CreateOneAsync(classroomOffering);

//    var major = new Major { Id = "Major01", Name = "Science", SchoolId = school.Id };
//    await GenericDataService<Major>.Instance.CreateOneAsync(major);

//    var subject = new Subject { Id = "Subject01", Name = "Physics", MajorId = major.Id };
//    await GenericDataService<Subject>.Instance.CreateOneAsync(subject);

//    var subjectOffering = new SubjectOffering { Id = "SubjectOffering01", SubjectId = subject.Id, SchoolYearId = schoolYear.Id };
//    await GenericDataService<SubjectOffering>.Instance.CreateOneAsync(subjectOffering);

//    var scoreType = new ScoreType { Id = "ScoreType01", Name = "Exam", Coefficient = 1.0, SchoolId = school.Id };
//    await GenericDataService<ScoreType>.Instance.CreateOneAsync(scoreType);

//    var subjectScoreType = new SubjectScoreType { Id = "SubjectScoreType01", ScoreTypeId = scoreType.Id, SubjectOfferingId = subjectOffering.Id };
//    await GenericDataService<SubjectScoreType>.Instance.CreateOneAsync(subjectScoreType);

//    var user = new User { Id = "User01", Username = "admin", Password = "password", Email = "admin@schoola.com", Role = "Admin", SchoolId = school.Id };
//    await GenericDataService<User>.Instance.CreateOneAsync(user);

//    var teacher = new Teacher { Id = "Teacher01", Username = "teacherA", Password = "password", Email = "teacherA@schoola.com", Role = "Teacher", SchoolId = school.Id, FirstName = "Alice", LastName = "Smith", MajorId = major.Id };
//    await GenericDataService<Teacher>.Instance.CreateOneAsync(teacher);

//    var student = new Student { Id = "Student01", FirstName = "John", LastName = "Doe", Gender = "Male", Address = "123 Main St", Birthday = new DateTime(2010, 5, 15), Phone = "123-456-7890", Ethnicity = "Ethnicity A", Religion = "Religion A", Email = "john.doe@schoola.com", SchoolId = school.Id };
//    await GenericDataService<Student>.Instance.CreateOneAsync(student);

//    var studentAssignment = new StudentAssignment { Id = "StudentAssignment01", Semester = 1, Conduct = "Good", ExcusedAbsence = 2, UnexcusedAbsence = 0, StudentId = student.Id, ClassroomOfferingId = classroomOffering.Id };
//    await GenericDataService<StudentAssignment>.Instance.CreateOneAsync(studentAssignment);

//    var teacherAssignment = new TeacherAssignment { Id = "TeacherAssignment01", Weekday = "Monday", Period = 1, TeacherId = teacher.Id, SubjectOfferingId = subjectOffering.Id, ClassroomOfferingId = classroomOffering.Id };
//    await GenericDataService<TeacherAssignment>.Instance.CreateOneAsync(teacherAssignment);

//    var score = new Score { Id = "Score01", Value = 95.0, RecordNo = 1, StudentAssignmentId = studentAssignment.Id, SubjectScoreTypeId = subjectScoreType.Id };
//    await GenericDataService<Score>.Instance.CreateOneAsync(score);
//}
