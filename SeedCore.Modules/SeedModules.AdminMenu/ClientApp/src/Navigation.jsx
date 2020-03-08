import React, { useRef, useEffect } from 'react';
import { Layout } from 'antd';

export default props => {
  const contentBody = useRef();

  useEffect(() => {
    props.body.forEach(node => {
      if (contentBody.current) {
        contentBody.current.appendChild(node);
      }
    });

    // if (props.site.__proto__ === Promise.prototype) {
    //   props.site.then(result => {
    //     setSite(result);
    //     setDefaultModule(result);
    //   });
    // } else {
    //   setSite(props.site);
    //   setDefaultModule(props.site);
    // }
  }, []);

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
