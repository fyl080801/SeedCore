using System;

namespace SeedCore.SpaService.Core
{
    public class SpaProjectAttribute : Attribute
    {
        private string _path;
        private string _project;

        public SpaProjectAttribute(string project, string path)
        {
            _path = path;
            _project = project;
        }

        public string Path { get { return _path; } }

        public string Project { get { return _project; } }
    }
}