import React, { ReactNode } from 'react';
import Text from "@mui/material/TextField"

type HomeLayoutProps = {
  children: ReactNode;
}

const HomeLayout = ({children}: HomeLayoutProps)  => { return (
  <Text>Welcome to the home page</Text>
  );
};

export default HomeLayout;