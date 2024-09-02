using UnityEngine;
using UnityEditor;

namespace DuksGames.Tools
{
    public class CommandImportException : System.Exception
    {
        public CommandImportException() {
        }

        public CommandImportException(string msg) : base(msg) {
        }

        public CommandImportException(string msg, System.Exception inner) : base(msg, inner) {
        }

        public CommandImportException(string msg, StarterWorkTicketData starterWorkTicketData) : 
            base($"{msg} | {starterWorkTicketData.Dump()}") {
        } 
        
        public CommandImportException(string msg, StarterWorkTicketData starterWorkTicketData, System.Exception inner) : 
            base($"{msg} | {starterWorkTicketData.Dump()}", inner) {

        } 
    }
}