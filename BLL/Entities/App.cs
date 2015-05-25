using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Entities
{
    public class App
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
        public string CameraId { get; set; }
        public string CameraUrl { get; set; }
        public string CameraUser { get; set; }
        public string CameraPassword { get; set; }
        public string Title { get; set; }
        public string ThumbFile { get; set; }
        public string IconFile { get; set; }
        public string AppFile { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
