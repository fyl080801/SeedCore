import React, { useState, useEffect } from 'react';
import {
  Typography,
  Button,
  Form,
  Input,
  Select,
  Row,
  Col,
  Modal,
  Spin,
  Icon
} from 'antd';
import {
  getDatabaseProviders,
  getTimeZones,
  getRecipes,
  postExecute
} from './apis/setup';
import logo from './logo.svg';

import 'bootstrap/dist/css/bootstrap-grid.css';
import 'antd/dist/antd.css';
import './App.css';

export default Form.create()(props => {
  const [providers, setProviders] = useState([]);
  const [timeZones, setTimeZones] = useState([]);
  const [recipes, setRecipes] = useState([]);

  const install = async () => {
    validateFields(async (errors, values) => {
      const hasError = Object.keys(errors).find(item => item.length > 0);
      if (hasError) {
        return;
      }

      var modal = Modal.warning({
        icon: null,
        content: (
          <Spin spinning={true} tip="提交中...">
            <div style={{ width: '100%' }}></div>
          </Spin>
        ),
        okButtonProps: { hidden: true }
      });

      try {
        await postExecute(getFieldsValue());
        window.location.href = window.location.href;
        window.location.reload();
      } catch {
        modal.destroy();
      }
    });
  };

  useEffect(() => {
    getDatabaseProviders().then(result => {
      setProviders(result);
    });

    getTimeZones().then(result => {
      setTimeZones(result);
    });

    getRecipes().then(result => {
      setRecipes(result);
    });
  }, []);

  const {
    getFieldDecorator,
    getFieldsValue,
    getFieldValue,
    validateFields
  } = props.form;

  return (
    <div className="container">
      <div className="jumbotron mt-5">
        <img src={logo} style={{ float: 'left' }} width="126px"></img>
        <Typography.Title level={1}>设置</Typography.Title>
        <Typography.Text>请填写系统设置相关信息</Typography.Text>
      </div>
      <Form labelCol={120}>
        <Row gutter={24}>
          <Col span={24}>
            <Typography.Title level={3}>
              <Icon type="setting" /> 基本信息
            </Typography.Title>
            <hr />
          </Col>
          <Col span={8}>
            <Form.Item label="名称:">
              {getFieldDecorator('siteName', {
                rules: [{ required: true, message: '必填' }]
              })(<Input placeholder="输入名称" />)}
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item label="产品:">
              <Input.Group compact>
                {getFieldDecorator('recipeName', {
                  rules: [{ required: true, message: '必填' }]
                })(
                  <Select placeholder="请选择" style={{ width: '60%' }}>
                    {recipes.map((item, index) => (
                      <Select.Option key={index} value={item.name}>
                        {item.displayName}
                      </Select.Option>
                    ))}
                  </Select>
                )}
                <Button icon="folder-open">打开</Button>
              </Input.Group>
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item label="时区:">
              {getFieldDecorator('siteTimeZone', {
                rules: [{ required: true, message: '必选' }]
              })(
                <Select
                  placeholder="请选择"
                  showSearch
                  filterOption={(input, option) =>
                    option.props.children
                      .toLowerCase()
                      .indexOf(input.toLowerCase()) >= 0
                  }
                >
                  {timeZones.map((item, index) => (
                    <Select.Option key={index} value={item.timeZoneId}>
                      {item.timeZoneName}
                    </Select.Option>
                  ))}
                </Select>
              )}
            </Form.Item>
          </Col>
          <Col span={24}>
            <Typography.Title level={3}>
              <Icon type="setting" /> 数据库
            </Typography.Title>
            <hr />
          </Col>
          <Col span={12}>
            <Form.Item label="类型:">
              {getFieldDecorator('databaseProvider', {
                rules: [{ required: true, message: '必选' }]
              })(
                <Select placeholder="请选择">
                  {providers.map((item, index) => (
                    <Select.Option key={index} value={item.provider}>
                      {item.name}
                    </Select.Option>
                  ))}
                </Select>
              )}
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="表前缀:">
              <Input placeholder="请输入" />
            </Form.Item>
          </Col>
          <Col span={24}>
            <Form.Item
              label="连接字符串:"
              extra={
                providers.find(
                  item => item.provider === getFieldValue('databaseProvider')
                )?.sample
              }
            >
              {getFieldDecorator('connectionString', {
                rules: [{ required: true, message: '必填' }]
              })(<Input placeholder="请输入" />)}
            </Form.Item>
          </Col>
          <Col span={24}>
            <Typography.Title level={3}>
              <Icon type="setting" /> 管理员信息
            </Typography.Title>
            <hr />
          </Col>
          <Col span={12}>
            <Form.Item label="用户名:">
              {getFieldDecorator('userName', {
                rules: [{ required: true, message: '必填' }]
              })(<Input placeholder="请输入" />)}
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="邮箱:">
              {getFieldDecorator('email', {
                rules: [
                  { required: true, message: '必填' },
                  { type: 'email', message: '格式不正确' }
                ]
              })(<Input placeholder="请输入" />)}
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="密码:">
              {getFieldDecorator('password', {
                rules: [{ required: true, message: '必填' }]
              })(
                <Input.Password placeholder="请输入" visibilityToggle={false} />
              )}
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="确认密码:">
              {getFieldDecorator('passwordConfirmation', {
                rules: [
                  { required: true, message: '必填' },
                  {
                    validator: (rule, value, callback) => {
                      if (!value || value.trim() === '') {
                        callback();
                        return;
                      }

                      const pwd = getFieldValue('password');

                      if (pwd !== value) {
                        callback('密码不一致');
                        return;
                      }

                      callback();
                    }
                  }
                ]
              })(
                <Input.Password placeholder="请输入" visibilityToggle={false} />
              )}
            </Form.Item>
          </Col>
        </Row>
      </Form>
      <div style={{ textAlign: 'center' }} className="bottom mt-5">
        <Button type="primary" size="large" onClick={install}>
          开始安装
        </Button>
      </div>
    </div>
  );
});
