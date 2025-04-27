import React from 'react';
import SignUpLayout from './SignUpLayout'
import SignUpForm from './SignUpForm';
import SignUpFooter from './SignUpFooter';


const SignUpPage = () => {
  return (
    <SignUpLayout>
      <SignUpForm/>
      <SignUpFooter/>
    </SignUpLayout>
  )
}

export default SignUpPage;