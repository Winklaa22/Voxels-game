using UnityEngine;

namespace _File
{
    [System.Serializable]
    public class FileData
    {
        [SerializeField] private string _fileName;
        [SerializeField] private string _fileType;

        public string GetFilePathName()
        {
            return _fileName + "." + _fileType;
        }
    }
}
