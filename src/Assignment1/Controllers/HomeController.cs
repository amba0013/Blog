using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Assignment1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Collections;
using System.Text;

namespace Assignment1.Controllers
{
    public class HomeController : Controller
    {
        private Assignment2DataContext _assignment1DataContext;

        public HomeController(Assignment2DataContext assDC)
        {
            _assignment1DataContext = assDC;

        }

        public IActionResult Index()
        {
            return View(_assignment1DataContext.BlogPosts.ToList()); //	This action will display the list of blogPosts in your database
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateAccount(User user)
        {

            _assignment1DataContext.Users.Add(user);
            _assignment1DataContext.SaveChanges();
            return RedirectToAction("Login");  //d.	This action then returns not ‘View()’ but ‘RedirectToView(“Index”)’
                                               //should redirect it to Login page
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CheckAccount(User user)
        {
            // FirstName, Last Name and User Id in the Session
            var getUserId = (
                from u in _assignment1DataContext.Users
                where u.EmailAddress == user.EmailAddress && u.Password == user.Password
                select u.UserId
                ).Single();

            int id = Convert.ToInt32(getUserId);
            HttpContext.Session.SetInt32("UserId", id);

            var getFirstName = (
                from u in _assignment1DataContext.Users
                where u.EmailAddress == user.EmailAddress && u.Password == user.Password
                select u.FirstName
                ).Single();
            HttpContext.Session.SetString("FirstName", getFirstName);

            var getLastName = (
                from u in _assignment1DataContext.Users
                where u.EmailAddress == user.EmailAddress && u.Password == user.Password
                select u.LastName
                ).Single();
            HttpContext.Session.SetString("LastName", getLastName);

            var Email = (
                from u in _assignment1DataContext.Users
                where u.EmailAddress == user.EmailAddress && u.Password == user.Password
                select u.EmailAddress
                ).Single();
            HttpContext.Session.SetString("Email", Email);

            var RoleId = (
               from u in _assignment1DataContext.Users
               where u.EmailAddress == user.EmailAddress && u.Password == user.Password
               select u.RoleId
               ).Single();
            HttpContext.Session.SetInt32("RoleId", RoleId);


            var checkIt = (
                from u in _assignment1DataContext.Users
                where u.EmailAddress == user.EmailAddress && u.Password == user.Password
                select u
                );

            if (checkIt.Count() == 0 || checkIt == null)
                return RedirectToAction("Login");
            else
                return RedirectToAction("Index");  //need to work on here

        }


        public IActionResult AddBlogPost()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateBlog(BlogPost blogPost)
        {

            //get id from the session
            var id = HttpContext.Session.GetInt32("UserId");

            //set that id to the foriegn key in the blogpost to be used when inserting
            blogPost.UserId = id.Value;

            _assignment1DataContext.Add(blogPost);
            _assignment1DataContext.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult DisplayFullBlogPost(int id)
        {
            //returns only one blog 
            var getSpecificBlog = (from b in _assignment1DataContext.BlogPosts
                                   where b.BlogPostId == id
                                   select b
                                    ).FirstOrDefault();

            //list of comments
            var comments = (from c in _assignment1DataContext.Comments where c.BlogPostId == id select c).ToList();

            var photos = (from c in _assignment1DataContext.Photos where c.BlogPostId == id select c).ToList();

            /* */
            var blogPostVM = new BlogPostVM();
            blogPostVM.BlogPost = getSpecificBlog;
            blogPostVM.Comments = comments;
            blogPostVM.Photos = photos;
            return View(blogPostVM); //returning getSpecificUser instead of getSpecificBlog
        }

        public IActionResult Comment()
        {
            var id = HttpContext.Session.GetInt32("UserId");

            var comment = new Comment();
            comment.UserId = id.Value;
            comment.BlogPostId = Convert.ToInt32(HttpContext.Request.Form["BlogPostId"]);

            string getcomment = HttpContext.Request.Form["comment"];

            string[] words = getcomment.Split(' ');
            var badwords = (from w in _assignment1DataContext.BadWords select w).ToList();

            for (int i = 0; i < words.Count(); i++)
            {
                //now search the word from the database
                foreach (var badw in badwords)
                {
                    if (badw.Word.Equals(words[i]))
                    {
                        words[i] = "******";
                    }
                }

            }

            //convert back the array to one string.
            comment.Content = ConvertStringArrayToString(words);

            _assignment1DataContext.Add(comment);
            _assignment1DataContext.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult EditBlogPost(int id)
        {
            var blogToUpDate = (from b in _assignment1DataContext.BlogPosts where b.BlogPostId == id select b).FirstOrDefault();

            return View(blogToUpDate);

        }

        public IActionResult ModifyBlog(BlogPost blog)
        {
            var id = Convert.ToInt32(Request.Form["BlogPostId"]);

            var blogToUpdate = (from b in _assignment1DataContext.BlogPosts where b.BlogPostId == id select b).FirstOrDefault();
            blogToUpdate.Title = blog.Title;
            blogToUpdate.ShortDescription = blog.ShortDescription;
            blogToUpdate.Content = blog.Content;
            blogToUpdate.Posted = blog.Posted;
            blogToUpdate.IsAvailable = blog.IsAvailable;


            _assignment1DataContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult DeleteBlog(int id)
        {


            var getSpecificBlog = (from b in _assignment1DataContext.BlogPosts
                                   where b.BlogPostId == id
                                   select b
                                    ).FirstOrDefault();

            //get all the rows that blogpostid contains
            var getSpecificComment = (from b in _assignment1DataContext.Comments
                                      where b.BlogPostId == id
                                      select b
                                      ).ToList();
            var getSpecificPhotos = (from b in _assignment1DataContext.Photos
                                     where b.BlogPostId == id
                                     select b
                                      ).ToList();
            for (int i = 0; i < getSpecificComment.Count(); i++)
            {
                _assignment1DataContext.Comments.Remove(getSpecificComment[i]);
                _assignment1DataContext.SaveChanges();
            }
            for (int i = 0; i < getSpecificPhotos.Count(); i++)
            {
                _assignment1DataContext.Photos.Remove(getSpecificPhotos[i]);
                _assignment1DataContext.SaveChanges();
            }

            _assignment1DataContext.Remove(getSpecificBlog);
            _assignment1DataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditProfile(int id)
        {

            var userToUpDate = (from b in _assignment1DataContext.Users where b.UserId == id select b).FirstOrDefault();

            return View(userToUpDate);

        }

        public IActionResult ModifyUser(User user)
        {
            var id = Convert.ToInt32(Request.Form["UserId"]);
            var userToUpdate = (from b in _assignment1DataContext.Users where b.UserId == id select b).FirstOrDefault();

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.EmailAddress = user.EmailAddress;
            userToUpdate.Password = user.Password;
            userToUpdate.DateOfBirth = user.DateOfBirth;
            userToUpdate.City = user.City;
            userToUpdate.Address = user.Address;
            userToUpdate.PostalCode = user.PostalCode;
            userToUpdate.Country = user.Country;
            userToUpdate.RoleId = user.RoleId;

            //save all changes
            _assignment1DataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileNow(ICollection<IFormFile> files)
        {

            // get your storage accounts connection string
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=cst8359;AccountKey=ecMPpNU6vimZKMDTJG4seALrY7Kq7UJYjgl0/yLanXn857C8xtUJ2sF4ciB6wy9gg+e/YeYbRTaly2DVOxWhXQ==");

            // create an instance of the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();

            // create a container to hold your blob (binary large object.. or something like that)
            // naming conventions for the curious https://msdn.microsoft.com/en-us/library/dd135715.aspx
            var container = blobClient.GetContainerReference("charlesstorage");
            await container.CreateIfNotExistsAsync();

            // set the permissions of the container to 'blob' to make them public
            var permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
            await container.SetPermissionsAsync(permissions);

            // for each file that may have been sent to the server from the client
            foreach (var file in files)
            {
                try
                {
                    // create the blob to hold the data
                    var blockBlob = container.GetBlockBlobReference(file.FileName);
                    if (await blockBlob.ExistsAsync())
                        await blockBlob.DeleteAsync();

                    using (var memoryStream = new MemoryStream())
                    {
                        // copy the file data into memory
                        await file.CopyToAsync(memoryStream);

                        // navigate back to the beginning of the memory stream
                        memoryStream.Position = 0;

                        // send the file to the cloud
                        await blockBlob.UploadFromStreamAsync(memoryStream);
                    }


                    // add the photo to the database if it uploaded successfully
                    var photo = new Photo();
                    photo.Url = blockBlob.Uri.AbsoluteUri;
                    photo.FileName = file.FileName;
                    photo.BlogPostId = Convert.ToInt32(HttpContext.Request.Form["BlogPostId"]);

                    _assignment1DataContext.Photos.Add(photo);

                    _assignment1DataContext.SaveChanges();

                }
                catch
                {

                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeletePhoto(int id)
        {

            var getSpecificPhoto = (from b in _assignment1DataContext.Photos
                                    where b.PhotoId == id
                                    select b
                                  ).FirstOrDefault();

            _assignment1DataContext.Remove(getSpecificPhoto);
            _assignment1DataContext.SaveChanges();
            return RedirectToAction("index");

        }

        public IActionResult ViewBadWords()
        {
            return View(_assignment1DataContext.BadWords.ToList());
        }

        public IActionResult DeleteBadword(int id)
        {
            var getSpecificBadWord = (from b in _assignment1DataContext.BadWords
                                      where b.BadWordId == id
                                      select b
                                    ).FirstOrDefault();

            _assignment1DataContext.Remove(getSpecificBadWord);
            _assignment1DataContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult AddWord()
        {
            return View();
        }

        public IActionResult AddBadWordTOList(BadWord badword)
        {

            _assignment1DataContext.BadWords.Add(badword);
            _assignment1DataContext.SaveChanges();

            return RedirectToAction("ViewBadWords");

        }

        /*
         This will be used to convert an array to String      
        */
        static string ConvertStringArrayToString(string[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append(' ');
            }
            return builder.ToString();
        }
    }
}
