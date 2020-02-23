import React, { useState, useEffect } from 'react';
import { Typography, Button, Form, Input, Select, Row, Col } from 'antd';
import { getDatabaseProviders, postExecute } from './apis/setup';

import 'bootstrap/dist/css/bootstrap-grid.css';
import 'antd/dist/antd.css';
import './App.css';

export default () => {
  const [siteName, setSiteName] = useState('');
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [databaseProvider, setDatabaseProvider] = useState('');
  const [connectionString, setConnectionString] = useState('');

  const [providers, setProviders] = useState([]);

  const install = async () => {
    const result = await postExecute({
      siteName,
      userName,
      email,
      databaseProvider,
      connectionString
    });
    console.log(result);
  };

  useEffect(() => {
    getDatabaseProviders().then(result => {
      setProviders(result);
    });
  }, []);

  return (
    <div className="container">
      <div className="jumbotron mt-5">
        <Typography.Title level={1}>设置</Typography.Title>
        <Typography.Text>请填写系统设置相关信息</Typography.Text>
      </div>
      <Form labelCol={120}>
        <Row gutter={24}>
          <Col span={24}>
            <Typography.Title level={3}>基本信息</Typography.Title>
            <hr />
          </Col>
          <Col span={8}>
            <Form.Item label="名称:">
              <Input
                placeholder="输入名称"
                value={siteName}
                onChange={value => setSiteName(value.target.value)}
              />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item label="名称:">
              <Input.Group compact>
                <Select placeholder="请选择" style={{ width: '60%' }}></Select>
                <Button icon="folder-open">打开</Button>
              </Input.Group>
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item label="时区:">
              <Select placeholder="请选择" />
            </Form.Item>
          </Col>
          <Col span={24}>
            <Typography.Title level={3}>数据库</Typography.Title>
            <hr />
          </Col>
          <Col span={12}>
            <Form.Item label="类型:">
              <Select
                placeholder="请选择"
                value={databaseProvider}
                onChange={value => setDatabaseProvider(value)}
              >
                {providers.map((item, index) => (
                  <Select.Option key={index} value={item.provider}>
                    {item.name}
                  </Select.Option>
                ))}
              </Select>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="表前缀:">
              <Input placeholder="请输入" />
            </Form.Item>
          </Col>
          <Col span={24}>
            <Form.Item label="连接字符串:">
              <Input
                placeholder="请输入"
                value={connectionString}
                onChange={evt => setConnectionString(evt.target.value)}
              />
            </Form.Item>
          </Col>
          <Col span={24}>
            <Typography.Title level={3}>管理员信息</Typography.Title>
            <hr />
          </Col>
          <Col span={12}>
            <Form.Item label="用户名:">
              <Input
                placeholder="请输入"
                value={userName}
                onChange={evt => setUserName(evt.target.value)}
              />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="邮箱:">
              <Input
                placeholder="请输入"
                value={email}
                onChange={evt => setEmail(evt.target.value)}
              />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="密码:">
              <Input.Password placeholder="请输入" visibilityToggle={false} />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="确认密码:">
              <Input.Password placeholder="请输入" visibilityToggle={false} />
            </Form.Item>
          </Col>
        </Row>
      </Form>
      <Button type="primary" onClick={install}>
        开始安装
      </Button>
    </div>
  );
};
