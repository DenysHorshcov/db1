-- Користувачі
create table users (
    id                  bigserial primary key,
    email               varchar(255) not null unique,
    password_hash       varchar(255) not null,
    display_name        varchar(100) not null,
    user_type           varchar(20) not null
        check (user_type in ('client', 'freelancer', 'admin')),

    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint,
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Ролі
create table roles (
    id          smallserial primary key,
    name        varchar(50) not null unique,
    description text
);

-- Зв'язок користувач-роль
create table user_roles (
    user_id bigint not null references users(id),
    role_id smallint not null references roles(id),
    primary key (user_id, role_id)
);

-- Профіль фрілансера (1:1 з users)
create table freelancer_profiles (
    user_id             bigint primary key references users(id),
    title               varchar(150),
    hourly_rate         numeric(10,2),
    overview            text,
    country             varchar(100),
    experience_level    varchar(20)
        check (experience_level in ('junior', 'middle', 'senior')),

    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint
);

-- Профіль клієнта (1:1 з users)
create table client_profiles (
    user_id             bigint primary key references users(id),
    company_name        varchar(150),
    company_website     varchar(255),
    description         text,

    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint
);

-- Скіли
create table skills (
    id          serial primary key,
    name        varchar(100) not null unique,
    category    varchar(100)
);

-- Скіли користувача (N:M)
create table user_skills (
    user_id bigint not null references users(id),
    skill_id int not null references skills(id),
    skill_level smallint not null check (skill_level between 1 and 5),
    primary key (user_id, skill_id)
);

-- Категорії проєктів
create table project_categories (
    id      serial primary key,
    name    varchar(100) not null unique
);

-- Проєкти
create table projects (
    id                  bigserial primary key,
    client_id           bigint not null references users(id),
    category_id         int references project_categories(id),
    title               varchar(200) not null,
    description         text,
    budget_type         varchar(20) not null
        check (budget_type in ('fixed', 'hourly')),
    budget_min          numeric(12,2),
    budget_max          numeric(12,2),
    status              varchar(20) not null
        check (status in ('draft','open','in_progress','completed','cancelled')),
    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint,
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Пропозиції фрілансерів по проєкту
create table proposals (
    id                  bigserial primary key,
    project_id          bigint not null references projects(id),
    freelancer_id       bigint not null references users(id),
    cover_letter        text,
    proposed_budget     numeric(12,2),
    status              varchar(20) not null
        check (status in ('sent','shortlisted','rejected','accepted','withdrawn')),
    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint,
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Контракти
create table contracts (
    id                  bigserial primary key,
    project_id          bigint not null references projects(id),
    client_id           bigint not null references users(id),
    freelancer_id       bigint not null references users(id),
    proposal_id         bigint references proposals(id),
    start_date          date not null,
    end_date            date,
    status              varchar(20) not null
        check (status in ('active','on_hold','completed','cancelled')),
    hourly_rate         numeric(12,2),

    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint,
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Етапи (milestones)
create table milestones (
    id                  bigserial primary key,
    contract_id         bigint not null references contracts(id),
    title               varchar(200) not null,
    description         text,
    due_date            date,
    amount              numeric(12,2) not null,
    status              varchar(20) not null
        check (status in ('pending','in_review','approved','paid')),

    created_at          timestamptz not null default now(),
    created_by_user_id  bigint,
    updated_at          timestamptz,
    updated_by_user_id  bigint,
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Платежі
create table payments (
    id              bigserial primary key,
    milestone_id    bigint references milestones(id),
    contract_id     bigint not null references contracts(id),
    payer_id        bigint not null references users(id),
    payee_id        bigint not null references users(id),
    amount          numeric(12,2) not null,
    paid_at         timestamptz,
    method          varchar(50),
    status          varchar(20) not null
        check (status in ('pending','completed','failed','refunded'))
);

-- Відгуки
create table reviews (
    id                  bigserial primary key,
    contract_id         bigint not null references contracts(id),
    from_user_id        bigint not null references users(id),
    to_user_id          bigint not null references users(id),
    rating              smallint not null check (rating between 1 and 5),
    comment             text,
    created_at          timestamptz not null default now(),
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Повідомлення
create table messages (
    id                  bigserial primary key,
    project_id          bigint references projects(id),
    contract_id         bigint references contracts(id),
    from_user_id        bigint not null references users(id),
    to_user_id          bigint not null references users(id),
    body                text not null,
    sent_at             timestamptz not null default now(),
    is_read             boolean not null default false,
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);

-- Вкладення
create table attachments (
    id              bigserial primary key,
    message_id      bigint references messages(id),
    project_id      bigint references projects(id),
    file_name       varchar(255) not null,
    file_url        varchar(500) not null,
    content_type    varchar(100)
);

-- Нотифікації
create table notifications (
    id                  bigserial primary key,
    user_id             bigint not null references users(id),
    type                varchar(50) not null,
    payload             jsonb,
    is_read             boolean not null default false,
    created_at          timestamptz not null default now(),
    is_deleted          boolean not null default false,
    deleted_at          timestamptz,
    deleted_by_user_id  bigint
);
