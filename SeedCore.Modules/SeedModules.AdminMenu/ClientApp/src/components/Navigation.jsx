import React, { useRef, useEffect } from 'react';
import BasicLayout from '@ant-design/pro-layout/es/BasicLayout';
import RightContent from './Header/RightContent';

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
    <BasicLayout title="test" rightContentRender={() => <RightContent />}>
      <div ref={contentBody} />
    </BasicLayout>
  );
};
