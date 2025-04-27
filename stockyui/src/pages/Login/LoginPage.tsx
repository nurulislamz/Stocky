import React from 'react';
import LoginLayout from './LoginLayout';
import LoginForm from './LoginForm';
import LoginFooter from './LoginFooter';

const LoginPage = () => {
  return (
    <LoginLayout>
      <LoginForm/>
      <LoginFooter/>
    </LoginLayout>
  )
}

export default LoginPage;