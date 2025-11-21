// DoctestTestSuiteKeyword.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal sealed class DoctestTestSuiteKeyword : Keyword
    {
        private List<string> _allTestSuiteNames = new List<string>();
        private Stack<string> _testSuiteStack = new Stack<string>();

        internal override string Word => "TEST_SUITE";

        internal DoctestTestSuiteKeyword(List<string> allTestSuiteNames)
        {
            _allTestSuiteNames = allTestSuiteNames;
        }

        internal void Reset()
        {
            _testSuiteStack.Clear();
        }

        internal override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            string testSuiteName = _allTestSuiteNames.Find(s => line.Contains("\"" + s + "\""));

            if (string.IsNullOrEmpty(testSuiteName))
            {
                _testSuiteStack.Push(string.Empty); // Push placeholder to balance stack
                return;
            }

            _testSuiteStack.Push(testSuiteName);

            // Use className for TEST_SUITE to keep it separate from C++ namespaces
            if (string.IsNullOrEmpty(className))
            {
                className = testSuiteName;
            }
            else
            {
                className += (doubleColonSeparator + testSuiteName);
            }
        }

        internal override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            if (_testSuiteStack.Count == 0)
            {
                return;
            }

            string exitingSuite = _testSuiteStack.Pop();
            if (string.IsNullOrEmpty(exitingSuite))
            {
                return;
            }

            // Rebuild className from remaining stack
            var remainingSuites = _testSuiteStack.Reverse().Where(s => !string.IsNullOrEmpty(s));
            className = string.Join(doubleColonSeparator, remainingSuites);
        }
    }
}
