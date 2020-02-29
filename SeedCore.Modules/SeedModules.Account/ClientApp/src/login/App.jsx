import React from 'react';
import { Modal, Button, Form, Input, Checkbox } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import cookie from 'react-cookies';
import 'bootstrap/dist/css/bootstrap-grid.css';
import 'antd/dist/antd.css';
import './App.css';
import { useState } from 'react';
import { useRef } from 'react';

export default () => {
  const [formData, setFormData] = useState({});
  const form = useRef();
  const login = values => {
    setFormData(values);
    form.current.submit();
  };

  return (
    <div>
      <form
        ref={form}
        method="POST"
        action={`/SeedModules.Account/Account/Login${window.location.search}`}
        style={{ visibility: 'collapse' }}
      >
        <input type="hidden" name="userName" value={formData.userName} />
        <input type="hidden" name="password" value={formData.password} />
        <input type="hidden" name="rememberMe" value={formData.rememberMe} />
        <input
          type="hidden"
          name={cookie.load('XSRF-FIELD')}
          value={cookie.load('XSRF-TOKEN')}
        />
      </form>
      <Modal
        centered={true}
        visible={true}
        closable={false}
        className="components-form-normal-login"
        width={300}
        title="用户登录"
        footer={null}
      >
        <Form
          // form={form}

          name="normal_login"
          className="login-form"
          initialValues={{
            rememberMe: true,
            [cookie.load('XSRF-FIELD')]: cookie.load('XSRF-TOKEN')
          }}
          onFinish={login}
        >
          <Form.Item
            name="userName"
            rules={[{ required: true, message: '请输入用户名' }]}
          >
            <Input
              prefix={<UserOutlined className="site-form-item-icon" />}
              placeholder="用户名"
            />
          </Form.Item>
          <Form.Item
            name="password"
            rules={[{ required: true, message: '请输入密码' }]}
          >
            <Input
              prefix={<LockOutlined className="site-form-item-icon" />}
              type="password"
              placeholder="密码"
            />
          </Form.Item>
          <div className="ant-row ant-form-item" style={{ display: 'block' }}>
            <Form.Item name="rememberMe" valuePropName="checked" noStyle>
              <Checkbox>记住用户</Checkbox>
            </Form.Item>
            <a className="login-form-forgot" href="">
              忘记密码
            </a>
          </div>
          <Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              className="login-form-button"
            >
              登录
            </Button>
            或 <a href="">立即注册</a>
          </Form.Item>
          <Form.Item name={cookie.load('XSRF-FIELD')} noStyle>
            <Input type="hidden" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};
