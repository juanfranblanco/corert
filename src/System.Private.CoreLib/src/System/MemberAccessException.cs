// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

////////////////////////////////////////////////////////////////////////////////
// MemberAccessException
// Thrown when we try accessing a member that we cannot
// access, due to it being removed, private or something similar.
////////////////////////////////////////////////////////////////////////////////

using System;

namespace System
{
    // The MemberAccessException is thrown when trying to access a class
    // member fails.
    // 
    [System.Runtime.InteropServices.ComVisible(true)]
    public class MemberAccessException : Exception
    {
        // Creates a new MemberAccessException with its message string set to
        // the empty string, its HRESULT set to COR_E_MEMBERACCESS, 
        // and its ExceptionInfo reference set to null. 
        public MemberAccessException()
            : base(SR.Arg_AccessException)
        {
            SetErrorCode(__HResults.COR_E_MEMBERACCESS);
        }

        // Creates a new MemberAccessException with its message string set to
        // message, its HRESULT set to COR_E_ACCESS, 
        // and its ExceptionInfo reference set to null. 
        // 
        public MemberAccessException(String message)
            : base(message)
        {
            SetErrorCode(__HResults.COR_E_MEMBERACCESS);
        }

        public MemberAccessException(String message, Exception inner)
            : base(message, inner)
        {
            SetErrorCode(__HResults.COR_E_MEMBERACCESS);
        }
    }
}