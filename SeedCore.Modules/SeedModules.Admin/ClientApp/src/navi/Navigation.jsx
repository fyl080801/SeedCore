import React, { useRef } from 'react';
import { Layout } from 'antd';

export default () => {
  const contentBody = useRef();

  return (
    <Layout className="navi">
      <Layout.Header>
        <div className="logo" />
      </Layout.Header>
      <Layout>
        <Layout.Sider className="flex"></Layout.Sider>
        <Layout>
          <Layout.Content>
            <div ref={contentBody} />
          </Layout.Content>
        </Layout>
      </Layout>
    </Layout>
  );
};
